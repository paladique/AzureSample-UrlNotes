using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using UrlNotes.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace UrlNotes
{
    public static class DocumentDBRepository
    {
        private static DocumentClient client;
        private static Dictionary<string, string> config;


        static DocumentDBRepository()
        {
            GetConfiguration();
            client = new DocumentClient(new Uri(config["endpoint"]), config["authkey"]);
        }

        public static async Task<string> GetAllDocs<T>(string collectionName)
        {
            var q = client.CreateDocumentQuery<T>(GetCollectionUri(collectionName), new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Select(item => item)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (q.HasMoreResults)
            {
                results.AddRange(await q.ExecuteNextAsync<T>());
            }

            return JsonConvert.SerializeObject(results);
        }

        public static async Task<List<string>> GetDBCollections()
        {
            bool dbExists = CheckDBExists();

            if (!dbExists)
            {
                CreateDB().Wait();
                await CreateCollections();
                InsertDocuments();
            }

            IEnumerable<string> collectionNames;
            try
            {
                var colls = await client.ReadDocumentCollectionFeedAsync(UriFactory.CreateDatabaseUri(config["database"]));
                collectionNames = from x in colls
                                  select x.Id;
            }
            catch (Exception ex)
            {
                throw;
            }

            return collectionNames.ToList<string>();
        }

        private static async Task CreateCollections()
        {
            var videoCollection = await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(config["database"]), new DocumentCollection { Id = "Videos" });
            var docsCollectionn = await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(config["database"]), new DocumentCollection { Id = "Docs" });
        }

        private static bool CheckDBExists()
        {
            var dbId = config["database"] ?? "NotesDB";
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == dbId).ToArray().FirstOrDefault();

            if (database != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async void CreateDocument<T>(T item, string collectionName) where T : Item
        {
            await client.CreateDocumentAsync((GetCollectionUri(collectionName)), item);
        }

        public static string Search<T>(string collectionName, string searchTerms, string searchText)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            var terms = searchTerms.Trim().Split(' ');
            var contains = terms.Select(x => string.Format("CONTAINS({0}, @{1})", x == "keywords" ? "k.name" : "item." + x, x)).ToArray();
                         

            var q = new SqlQuerySpec
            {
                QueryText = "SELECT VALUE item FROM item JOIN k IN item.keywords WHERE " + contains[0],
                Parameters = new SqlParameterCollection()
                {
                    new SqlParameter("@" + terms[0].ToLower(), searchText)

                }
            };

            if (terms.Length > 2 && contains.Length > 2)
            {
                q.Parameters.Add(new SqlParameter("@" + terms[1], searchText));
                q.QueryText += " OR " + contains[1];
            }

            List<T> queryResult = (client.CreateDocumentQuery<T>(
                     GetCollectionUri(collectionName),
                     q,
                     queryOptions)).ToList();

            return JsonConvert.SerializeObject(queryResult);
        }



        public static async void DeleteDocument(string id, string collectionName)
        {
            var doc = GetDocumentItem<Item>(id, collectionName);
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(config["database"], collectionName, id));
        }

        public static async void EditDocument(Item item, string collectionName)
        {
            var doc = GetDocument(item.Id, collectionName);
            doc.SetPropertyValue("notes", item.Notes);
            doc.SetPropertyValue("tutorial", item.IsTutorial);

            Document updated = await client.ReplaceDocumentAsync(doc);
        }

        public static Document GetDocument(string id, string collectionName)
        {
            var doc = client.CreateDocumentQuery<Document>(GetCollectionUri(collectionName), new FeedOptions { EnableCrossPartitionQuery = true })
                                        .Where(r => r.Id == id)
                                        .AsEnumerable()
                                        .SingleOrDefault();
            return doc;
        }

        public static T GetDocumentItem<T>(string id, string collectionName) where T : Item
        {
            var docItem = client.CreateDocumentQuery<T>(GetCollectionUri(collectionName), new FeedOptions { EnableCrossPartitionQuery = true })
                                        .Where(r => r.Id == id)
                                        .AsEnumerable()
                                        .SingleOrDefault();
            return docItem;
        }

        public static void GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder = builder.AddJsonFile($"appsettings.ConnectionStrings.json", optional: true);
            builder = builder.AddEnvironmentVariables();
            var configBuilder = builder.Build();

            config = new Dictionary<string, string>
            {
                { "database", configBuilder.GetConnectionString("database") ?? "NotesDB" },
                { "authkey", configBuilder.GetConnectionString("authkey") },
                { "endpoint", configBuilder.GetConnectionString("endpoint") }
            };
        }

        public static Uri GetCollectionUri(string collectionName)
        {
            return UriFactory.CreateDocumentCollectionUri(config["database"], collectionName);
        }

        private async static Task CreateDB()
        {
            try
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "NotesDB" });
                config["database"] = database.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private static void InsertDocuments()
        {
            Video[] starterVideos =  {
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-1-of-4-Overview-of-existing-tools-to-create-Azure-Container-Service-clusters", IsTutorial = true, Notes = "First of four-part series on Azure Container Service" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-2-of-4-Create-an-Azure-Container-Instance-with-the-container-image", IsTutorial = true }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-3-of-4-Create-a-Web-App-for-Containers-and-set-up-a-web-hook", IsTutorial = true }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-4-of-4-Use-Azure-Container-Registry", IsTutorial = true }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Windows-Developer-Awards-Voting-Opens-AI-courses-from-Microsoft-Serial-Console-Access-in-Azure-" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Metaparticle", Notes = "Metaparticle sounds cool!" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Building-the-New-MSN-News-App-with-Xamarin" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/5-Things/Episode-12-Five-Things-About-Docker", IsTutorial = false }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Scalable--Service-Data-with-CosmosDB-for-Mobile", IsTutorial = false }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Tech-Crumbs/Starting-with-CosmosDB", IsTutorial = true }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Visual-Studio-Toolbox/CosmosDB-Serverless-NoSQL-for-the-NET-Developer" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/On-NET/Jeremy-Likness-CosmosDB-and-NET-Core", IsTutorial = true }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Whats-New-in-Azure-Cosmos-DB", Notes = "Info on what's happening with CosmosDB!" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Cosmos-DB-Comprehensive-Overview", IsTutorial = false }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Container-Instances-for-Multiplayer-Gaming-Theater", Notes = "Info on how to use containers with games" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Planning-and-building-games-using-the-full-power-of-VSTS-Agile-CI--CD-end-to-end-demo" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/Internet-of-Things-Show/Azure-IoT-Toolkit-extension-for-Visual-Studio-Code" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Connect-Next-Week-Picking-the-Right-Azure-VM-Star-Wars-Video-Cards-Life-After-the-Uniform-and-m" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Blogs/Azure/Log-Alerts-for-Application-Insights-Preview" }),
                new Video(new Item() { stringUrl = "https://channel9.msdn.com/Shows/AI-Show/The-Simplest-Machine-Learning" })
            };

            foreach (var item in starterVideos)
            {
                CreateDocument(item, "Videos");
            }

            var Docs = new List<Doc>();

            Doc[] starterDocs =  {
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid", IsTutorial = false, Notes = "Event Grid is very interesting! Look at this later"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-rest-api", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/scripts/app-service-cli-backup-onetime", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://azure.microsoft.com/en-us/overview/serverless-computing", IsTutorial = false, Notes = "Intro to serverless computing"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview", IsTutorial = false}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/query-cheat-sheet"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/20-days-of-tips", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/faq"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/manage-account"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-twitter-email"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-github-webhook-triggered-function", IsTutorial = true, Notes = "GitHub webhook triggered Azure Functions"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/scripts/functions-cli-create-function-app-vsts-continuous", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/durable-functions-overview"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings"}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/resize-images-on-storage-blob-upload-event", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/concepts", Notes = "Basic concepts on event grid.", IsTutorial = false}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp", Notes = "Quickstarts to get started with Azure Cognitive Services", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-serverless-api", IsTutorial = true}),
                new Doc(new Item() { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnetcore-get-started", IsTutorial = true})
            };


            foreach (var item in starterDocs)
            {
                CreateDocument(item, "Docs");
            }
        }
    }
}
