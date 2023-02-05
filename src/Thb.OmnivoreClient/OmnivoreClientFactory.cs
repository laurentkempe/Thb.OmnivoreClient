using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Thb.OmnivoreClient;

/// <summary>
///     A factory for creating <see cref="IOmnivoreClient" /> instances.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class OmnivoreClientFactory
{
    /// <summary>
    ///     Creates a new <see cref="OmnivoreClient" /> instance.
    /// </summary>
    /// <param name="apiUrl">The Omnivore GraphQL API URL.</param>
    /// <param name="authToken">The Omnivore API token.</param>
    /// <returns>The <see cref="OmnivoreClient" /> instance created.</returns>
    // ReSharper disable once UnusedMember.Global
    public static IOmnivoreClient Create(string apiUrl, string? authToken)
    {
        var graphQLClient = new GraphQLHttpClient(apiUrl, new SystemTextJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("Cookie", $"auth={authToken};");

        return new OmnivoreClient(graphQLClient);
    }
}