namespace Thb.OmnivoreClient;

public interface IOmnivoreClient
{
    /// <summary>
    /// Get the current user. 
    /// </summary>
    Task<OmnivoreClient.User> GetUserAsync();

    /// <summary>
    ///  Search for articles.
    /// </summary>
    Task<IEnumerable<OmnivoreClient.Node>> SearchAsync(string searchQuery = "");
}