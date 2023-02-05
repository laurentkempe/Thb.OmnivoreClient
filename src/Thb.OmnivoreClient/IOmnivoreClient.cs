namespace Thb.OmnivoreClient;

/// <summary>
///     A client for the Omnivore GraphQL API.
/// </summary>
public interface IOmnivoreClient
{
    /// <summary>
    ///     Get the current user.
    /// </summary>
    Task<User> GetUserAsync();

    /// <summary>
    ///     Search for articles.
    /// </summary>
    Task<IEnumerable<Node>> SearchAsync(string searchQuery = "");

    /// <summary>
    ///     Save a URL to Omnivore.
    /// </summary>
    /// <param name="user">Current <see cref="User" /> received from <see cref="OmnivoreClient.GetUserAsync" />.</param>
    /// <param name="url">The URL to be saved to Omnivore.</param>
    /// <returns><see cref="Uri" /> of the saved in Omnivore.</returns>
    /// <exception cref="InvalidOperationException">TODO Add a documentation when API is stabilized</exception>
    Task<Uri> SaveUrlAsync(User user, Uri url);
}