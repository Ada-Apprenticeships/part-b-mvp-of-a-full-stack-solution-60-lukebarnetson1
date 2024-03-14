using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

public class MongoDbService
{
    private readonly IMongoCollection<GameResponse> _gameResponses;

    public MongoDbService(string connectionString, string databaseName, string collectionName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _gameResponses = database.GetCollection<GameResponse>(collectionName);
    }

    public async Task StoreGameAsync(GameResponse gameData)
    {
        // generate new objectId for mongodb id format
        gameData.Id = ObjectId.GenerateNewId().ToString();

        // calculate the most common age rating and removing letters and punctuation 
        var ageRatings = gameData.Data.Ratings.Values
            .Where(rating => !string.IsNullOrEmpty(rating.Rating) && Regex.IsMatch(rating.Rating, @"^\d+$"))
            .Select(rating => int.Parse(rating.Rating))
            .ToList();

        if (ageRatings.Any())
        {
            var mostCommonAgeRating = ageRatings
                .GroupBy(rating => rating)
                .OrderByDescending(group => group.Count())
                .First()
                .Key;

            gameData.Data.MostCommonAgeRating = mostCommonAgeRating;
        }

        // store game data in mongodb
        await _gameResponses.ReplaceOneAsync(
            game => game.Id == gameData.Id,
            gameData,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<GameResponse> GetGameAsync(string gameId)
    {
        // get game data from mongodb
        var gameData = await _gameResponses.Find(game => game.Id == gameId).FirstOrDefaultAsync();

        return gameData;
    }

    public async Task<List<GameResponse>> GetAllGamesAsync()
    {
        // get all game data from mongodb
        var gameData = await _gameResponses.Find(game => true).ToListAsync();

        return gameData;
    }

    public async Task<List<GameResponse>> GetGamesForUserAsync(int userAge, int userBudget, string userLanguage)
    {
        // get all games
        var allGames = await GetAllGamesAsync();

        // filter games based on the user's age, the most common age rating, the user's budget, and the user's language
        var gamesForUser = allGames.Where(game =>
            game.Data.MostCommonAgeRating <= userAge &&
            (game.Data.PriceOverview == null || game.Data.PriceOverview.Final <= userBudget * 100) && // if priceOverview is null, the game must be free. multiply price by 100 because prices are in pence
            game.Data.SupportedLanguages.ToLower().Contains(userLanguage.ToLower()) // check if the game supports the user's language
        ).ToList();

        return gamesForUser;
    }

    public async Task ClearDatabaseAsync()
    {
        await _gameResponses.DeleteManyAsync(FilterDefinition<GameResponse>.Empty);
    }
}