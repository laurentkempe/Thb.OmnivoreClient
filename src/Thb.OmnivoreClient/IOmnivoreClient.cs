namespace Thb.OmnivoreClient;

/// <summary>
///     A client for the Omnivore GraphQL API.
/// </summary>
public interface IOmnivoreClient
{
    /// <summary>
    ///     Get the current user.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    Task<User> GetUserAsync();

    /// <summary>
    ///     Search for articles.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    Task<IEnumerable<Node>> SearchAsync(string searchQuery = "");

    /// <summary>
    ///     Save a URL to Omnivore.
    /// </summary>
    /// <param name="user">Current <see cref="User" /> received from <see cref="OmnivoreClient.GetUserAsync" />.</param>
    /// <param name="url">The URL to be saved to Omnivore.</param>
    /// <returns><see cref="Uri" /> of the saved content in Omnivore.</returns>
    /// <exception cref="InvalidOperationException">Saving the content in Omnivore failed.</exception>
    // ReSharper disable once UnusedMember.Global
    Task<Uri> SaveUrlAsync(User user, Uri url);
}