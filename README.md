# Url Notes
ASP.NET Web application that uses Azure Cosmos DB to manage favorite docs and videos, with the ability to add personal notes for each individual url.

[View the blog post here](https://blogs.msdn.microsoft.com/webdev/2018/04/27/cosmos-db-solves-common-data-challenges/?WT.mc_id=academic-0000-jasmineg)

[Try Cosmos DB](https://azure.microsoft.com/try/cosmosdb/?utm_source=github&utm_medium=github-sample-cosmos-link&WT.mc_id=academic-0000-jasmineg)

## How it's built
- ASP.NET Core 2.0 Razor Pages
- JQuery
- Bootstrap
- Cosmos DB SQL .NET API
- Built in Visual Studio

## How to run
1. [Create a Cosmos DB Database](https://docs.microsoft.com/azure/cosmos-db/create-sql-api-dotnet?WT.mc_id=academic-0000-jasmineg#create-a-database-account?WT.mc_id=codesamples-cosmosdb-jasmineg)
1. Open project in favorite editor or IDE of choice. 
1. Open `appsettings.json`, copy auth key and url endpoint from Cosmos DB in the Azure Portal, replace placeholders with those values.
1. Build and run. (Non VS users: use `dotnet run` on the command line in project root)
