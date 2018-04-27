# Url Notes
ASP.NET Web application that uses Azure Cosmos DB to manage favorite docs and videos, with the ability to add personal notes for each individual url.

[View the blog post here]( https://blogs.msdn.microsoft.com/webdev/)

[Try Cosmos DB](https://azure.microsoft.com/en-us/try/cosmosdb/?utm_source=github&utm_medium=github-sample-cosmos-link)

## How it's built
- ASP.NET Core 2.0 Razor Pages
- JQuery
- Bootstrap
- Cosmos DB SQL .NET API
- Built in Visual Studio

## How to run
1. [Create a Cosmos DB Database](https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet#create-a-database-account?WT.mc_id=codesamples-cosmosdb-jasmineg)
1. Open project in favorite editor of choice. 
1. Open `appsettings.json`, copy auth key and url endpoint from Cosmos DB in the Azure Portal, replace placeholders with those values.
1. Build and run. (use `dotnet run` on the command line in project root)
