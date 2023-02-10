using GraphQL;
using GraphQL.Client.Abstractions;

namespace Thb.OmnivoreClient;

internal sealed class OmnivoreClient : IOmnivoreClient
{
    private readonly IGraphQLClient _graphQLClient;

    public OmnivoreClient(IGraphQLClient graphQLClient)
    {
        _graphQLClient = graphQLClient;
    }

    public async Task<User> GetUserAsync()
    {
        var userQueryRequest = new GraphQLRequest
        {
            Query = @"
                query Viewer {
                  me {  
                    id
                    name
                    isFullUser
                    profile {
                      id
                      username
                      pictureUrl
                      bio
                    }
                  }
                }
            "
        };

        var userResponse = await _graphQLClient.SendQueryAsync<UserResponse>(userQueryRequest);

        return userResponse.Data.Me;
    }

    public async Task<IEnumerable<Node>> SearchAsync(string searchQuery = "")
    {
        string? after = null;

        var searchQueryRequest = new GraphQLRequest
        {
            Variables = new
            {
                after,
                first = 2,
                format = "markdown",
                includeContent = true,
                query = searchQuery
            },
            Query = """
            query Search(
              $after: String
              $first: Int
              $query: String
              $includeContent: Boolean
              $format: String
            ) {
              search(
                after: $after
                first: $first
                query: $query
                includeContent: $includeContent
                format: $format
              ) {
                ... on SearchSuccess {
                  edges {
                    node {
                      slug
                      url
                      originalArticleUrl
                      title
                      content
                    }
                  }
                  pageInfo {
                    hasNextPage
                    endCursor
                    totalCount
                  }
                }
                ... on SearchError {
                  errorCodes
                }
              }
            }
            """
        };

        var searchResponse = await _graphQLClient.SendQueryAsync<SearchResponse>(searchQueryRequest);

        return (searchResponse.Data.Search.Edges ?? Array.Empty<Edges>()).Select(e => e.Node);
    }

    public async Task<Uri> SaveUrlAsync(User user, Uri url)
    {
        var saveUrlRequest = new GraphQLRequest
        {
            Variables = new
            {
                input = new
                {
                    clientRequestId = user.Id,
                    source = "thb-omnivoreclient",
                    url = url.ToString()
                }
            },
            Query = """
                 mutation SaveUrl($input: SaveUrlInput!) {
                   saveUrl(input: $input) {
                     ... on SaveSuccess {
                       url
                     }
                     ... on SaveError {
                       errorCodes
                     }
                   }
                 }
                 """
        };

        var saveUrl = await _graphQLClient.SendMutationAsync<SaveUrlResponse>(saveUrlRequest);

        return Uri.TryCreate(saveUrl.Data.SaveUrl.Url, UriKind.Absolute, out var uri)
            ? uri
            : throw new InvalidOperationException("Invalid URL returned from Omnivore API");
    }

    private record UserResponse(User Me);

    private record SearchResponse(Search Search);

    private record Search(Edges[]? Edges, PageInfo? PageInfo);

    private record Edges(Node Node);

    private record PageInfo(bool HasNextPage, string? EndCursor, int TotalCount);

    private record SaveUrlResponse(SaveUrl SaveUrl);

    private record SaveUrl(string Url);
}