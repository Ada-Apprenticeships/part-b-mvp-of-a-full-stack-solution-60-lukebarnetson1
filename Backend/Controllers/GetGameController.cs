using Ada_Advanced_Programming_Part_B.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class GetGameController : ControllerBase
{
    private readonly ISteamApiRequest _steamAPIRequest;
    private readonly SteamChartsScraperService _scraper;
    private readonly MongoDbService _mongoDbService;

    public GetGameController(ISteamApiRequest steamAPIRequest, SteamChartsScraperService scraper, MongoDbService mongoDbService)
    {
        _steamAPIRequest = steamAPIRequest;
        _scraper = scraper;
        _mongoDbService = mongoDbService;
    }

    // for getting games from steam and storing in database
    [HttpGet("SteamGetGame", Name = "GetGame")]
    public async Task<IActionResult> GetGame()
    {
        // clear database beforehand to prevent duplicated entries
        await _mongoDbService.ClearDatabaseAsync();

        // get the ids of the top games from steam charts
        var gameIds = await _scraper.ScrapeTopGames();

        var results = new Dictionary<string, GameResponse>();
        foreach (var gameId in gameIds)
        {
            // make request to steam api, using the gameId to specify the game, and gb as the country code to ensure the response is in english and price is in gbp
            var response = await _steamAPIRequest.GetStoreAsync("appdetails", $"?appids={gameId}&cc=gb");

            // only get useful info
            var result = JsonConvert.DeserializeObject<Dictionary<string, GameResponse>>(response);

            // set the Id property to the steamAppId
            result[gameId].Id = result[gameId].Data.SteamAppId.ToString();

            results[gameId] = result[gameId];

            // store the result in mongodb
            await _mongoDbService.StoreGameAsync(result[gameId]);
        }

        // return result with only useful info
        return Ok(results);
    }

    // for getting all games from database
    [HttpGet("SteamGetGames", Name = "GetAllGames")]
    public async Task<IActionResult> GetAllGames([FromQuery] int age, [FromQuery] int budget, [FromQuery] string language)
    {
        var results = await _mongoDbService.GetGamesForUserAsync(age, budget, language);

        return Ok(results);
    }

    // for getting tailored games for user from database
    [HttpGet("GetGamesForUser", Name = "GetGamesForUser")]
    public async Task<IActionResult> GetGamesForUser([FromQuery] int age, [FromQuery] int budget, [FromQuery] string language)
    {
        var results = await _mongoDbService.GetGamesForUserAsync(age, budget, language);
        return Ok(results);
    }

}