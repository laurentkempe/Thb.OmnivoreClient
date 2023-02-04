using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Thb.OmnivoreClient;

/// <summary>
///   A factory for creating <see cref="OmnivoreClient"/> instances.
/// </summary>
public static class OmnivoreClientFactory
{
    /// <summary>
    ///  Creates a new <see cref="OmnivoreClient"/> instance.
    /// </summary>
    /// <param name="apiUrl">The Omnivore GraphQL API URL.</param>
    /// <param name="authToken">The Omnivore API token.</param>
    /// <returns>The <see cref="OmnivoreClient"/> instance created.</returns>
    public static OmnivoreClient Create(string apiUrl, string? authToken)
    {
        var graphQLClient = new GraphQLHttpClient(apiUrl, new SystemTextJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("Cookie", $"auth={authToken};");

        return new OmnivoreClient(graphQLClient);
    }
}