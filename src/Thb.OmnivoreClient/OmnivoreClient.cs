using GraphQL;
using GraphQL.Client.Abstractions;

namespace Thb.OmnivoreClient;

/// <summary>
///  A client for the Omnivore GraphQL API. 
/// </summary>
public sealed class OmnivoreClient
{
    private readonly IGraphQLClient _graphQLClient;

    /// <summary>
    ///  Creates a new <see cref="OmnivoreClient"/> instance.
    /// </summary>
    /// <param name="graphQLClient"><see cref="IGraphQLClient"/> used to query the Omnivore GraphQL API.</param>
    public OmnivoreClient(IGraphQLClient graphQLClient)
    {
        _graphQLClient = graphQLClient;
    }
    
    /// <summary>
    /// Get the current user. 
    /// </summary>
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

    /// <summary>
    ///  Search for articles.
    /// </summary>
    public async Task<IEnumerable<Node>> SearchAsync(string searchQuery = "")
    {
        string? after = null;

        var searchQueryRequest = new GraphQLRequest
        {
            Variables = new
            {
                after = after,
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

    private record UserResponse(User Me);
    private record SearchResponse(Search Search);
    private record Search(Edges[]? Edges, PageInfo? PageInfo);
    private record Edges(Node Node);

    public record User(string Id, string Name, bool IsFullUser, Profile Profile);
    public record Profile(string Id, string Username, string PictureUrl, string Bio);

    public record Node(string Slug, string Title, string Url, string OriginalArticleUrl, string Content);
    public record PageInfo(bool HasNextPage, string? EndCursor, int TotalCount);
}