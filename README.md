⚠️ Project discontinued because OmnivoreApp has been shut down.

[![NuGet](https://img.shields.io/nuget/v/Thb.OmnivoreClient.svg)](https://www.nuget.org/packages/Thb.OmnivoreClient/)
[![GitHub license](https://img.shields.io/github/license/laurentkempe/Thb.OmnivoreClient.svg)](https://github.com/laurentkempe/Thb.OmnivoreClient/blob/main/LICENSE)
[![publish](https://github.com/laurentkempe/Thb.OmnivoreClient/actions/workflows/publish.yml/badge.svg)](https://github.com/laurentkempe/Thb.OmnivoreClient/actions/workflows/publish.yml)

# Thb.OmnivoreClient

A .NET client for [Omnivore GraphQL API](https://omnivore.app/), the free, open source, read-it-later app for serious
readers.

# How to install

Install the NuGet package [`Thb.OmnivoreClient`](https://www.nuget.org/packages/Thb.OmnivoreClient/)

# How to use

Set `OMNIVORE_AUTH_TOKEN` environment variable to your Omnivore API token.
See [how to get your API token](https://docs.omnivore.app/integrations/api.html#getting-an-api-token).

## Create a client

```csharp
var apiUrl = Environment.GetEnvironmentVariable("OMNIVORE_API_URL") ?? "https://api-prod.omnivore.app/api/graphql";
var omnivoreClient = OmnivoreClientFactory.Create(apiUrl, Environment.GetEnvironmentVariable("OMNIVORE_AUTH_TOKEN"));
```

## GetUserAsync

```csharp
var me = await omnivoreClient.GetUserAsync();
Console.WriteLine(me.Name);
```

## SearchAsync

```csharp
var searchResults = await omnivoreClient.SearchAsync();

foreach (var searchResult in searchResults)
{
    Console.WriteLine($"Title: {searchResult.Title}");
    Console.WriteLine($"Url: {searchResult.Url}");
    Console.WriteLine($"OriginalArticleUrl: {searchResult.OriginalArticleUrl}");
    Console.WriteLine($"Slug: {searchResult.Slug}");
    Console.WriteLine();
}
```

# Development

## How to package locally

```bash
dotnet r pack
```
