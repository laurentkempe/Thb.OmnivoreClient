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

    public async Task<Result<User>> GetUserAsync()
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

        try
        {
            var userResponse = await _graphQLClient.SendQueryAsync<UserResponse>(userQueryRequest);

            return userResponse.Errors is null
                ? Result<User>.Success(userResponse.Data.Me)
                : Result<User>.Failure(userResponse.Errors.Select(e => e.Message).ToArray());
        }
        catch (Exception e)
        {
            return Result<User>.Failure(new[] { e.Message });
        }
    }

    public async Task<Result<IEnumerable<Node>>> SearchAsync(string searchQuery = "")
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

        try
        {
            var searchResponse = await _graphQLClient.SendQueryAsync<SearchResponse>(searchQueryRequest);

            return searchResponse.Errors is null
                ? Result<IEnumerable<Node>>.Success(
                    (searchResponse.Data.Search.Edges ?? Array.Empty<Edges>()).Select(e => e.Node))
                : Result<IEnumerable<Node>>.Failure(searchResponse.Errors.Select(e => e.Message).ToArray());
        }
        catch (Exception e)
        {
            return Result<IEnumerable<Node>>.Failure(new[] { e.Message });
        }
    }

    public async Task<Result<Uri>> SaveUrlAsync(User user, Uri url)
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

        try
        {
            var saveUrlResponse = await _graphQLClient.SendMutationAsync<SaveUrlResponse>(saveUrlRequest);

            if (saveUrlResponse.Errors is null)
            {
                return Uri.TryCreate(saveUrlResponse.Data.SaveUrl.Url, UriKind.Absolute, out var uri)
                    ? Result<Uri>.Success(uri)
                    : Result<Uri>.Failure(new[] { "Invalid URL returned from Omnivore API." });
            }

            return Result<Uri>.Failure(saveUrlResponse.Errors.Select(e => e.Message).ToArray());
        }
        catch (Exception e)
        {
            return Result<Uri>.Failure(new[] { e.Message });
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private record UserResponse(User Me);

    // ReSharper disable once ClassNeverInstantiated.Local
    private record SearchResponse(Search Search);

    // ReSharper disable once ClassNeverInstantiated.Local
    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record Search(Edges[]? Edges, PageInfo? PageInfo);

    // ReSharper disable once ClassNeverInstantiated.Local
    private record Edges(Node Node);

    // ReSharper disable once ClassNeverInstantiated.Local
    // ReSharper disable NotAccessedPositionalProperty.Local
    private record PageInfo(bool HasNextPage, string? EndCursor, int TotalCount);

    // ReSharper disable once ClassNeverInstantiated.Local
    private record SaveUrlResponse(SaveUrl SaveUrl);

    // ReSharper disable once ClassNeverInstantiated.Local
    private record SaveUrl(string Url);
}