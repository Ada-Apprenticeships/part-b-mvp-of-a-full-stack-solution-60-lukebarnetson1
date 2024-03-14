namespace Ada_Advanced_Programming_Part_B.Backend.Services
{
    public interface ISteamApiRequest
    {
        Task<string> GetStoreAsync(string asset, string method, string filter = "");
        Task<string> GetWebApiAsync(string asset, string method, string filter = "");
    }
}