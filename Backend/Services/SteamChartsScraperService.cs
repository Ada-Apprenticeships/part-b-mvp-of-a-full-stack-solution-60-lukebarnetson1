using HtmlAgilityPack;

namespace Ada_Advanced_Programming_Part_B.Backend.Services
{
    public class SteamChartsScraperService
    {
        private const string SteamChartsUrl = "http://steamcharts.com/top";

        public async Task<List<string>> ScrapeTopGames()
        {
            try
            {
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(SteamChartsUrl);

                // select all game rows
                var gameNodes = doc.DocumentNode.SelectNodes("//table[@id='top-games']//tr[position()>0 and position()<26]//a");

                if (gameNodes == null)
                {
                    Console.WriteLine("Error: Unable to find game nodes.");
                    return null;
                }

                var gameIds = new List<string>();
                foreach (var gameNode in gameNodes)
                {
                    // get href attribute value
                    var gameHref = gameNode.GetAttributeValue("href", "");

                    // extract id from href string
                    var gameId = gameHref.Replace("/app/", "");

                    gameIds.Add(gameId);
                }

                return gameIds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping top games: {ex.Message}");
                return null;
            }
        }
    }
}