using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using contextual_notes.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace contextual_notes
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static DocumentClient client;

        public static async Task<string> GetAllDocsAsync(string collection)
        {
            var c = GetConfiguration();

            client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
            var q = client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(c["database"], collection), new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Select(f => f)
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

            var c = GetConfiguration();
            IEnumerable<string> collectionNames;
            try
            {
                var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
                var colls = await client.ReadDocumentCollectionFeedAsync(UriFactory.CreateDatabaseUri(c["database"]));

                collectionNames = from x in colls
                                  select x.Id;

            }
            catch (Exception ex)
            {

                throw;
            }

            return collectionNames.ToList<string>();
        }

        private static void InsertDocuments()
        {
            Video[] starterVideos =  {
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-1-of-4-Overview-of-existing-tools-to-create-Azure-Container-Service-clusters", Tutorial = true, Comments = "First of four-part series on Azure Container Service" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-2-of-4-Create-an-Azure-Container-Instance-with-the-container-image", Tutorial = true },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-3-of-4-Create-a-Web-App-for-Containers-and-set-up-a-web-hook", Tutorial = true },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-4-of-4-Use-Azure-Container-Registry", Tutorial = true },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Windows-Developer-Awards-Voting-Opens-AI-courses-from-Microsoft-Serial-Console-Access-in-Azure-" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Metaparticle", Comments = "Metaparticle sounds cool!" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Building-the-New-MSN-News-App-with-Xamarin" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/5-Things/Episode-12-Five-Things-About-Docker", Tutorial = false },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Scalable--Service-Data-with-CosmosDB-for-Mobile", Tutorial = false },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Tech-Crumbs/Starting-with-CosmosDB", Tutorial = true },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Visual-Studio-Toolbox/CosmosDB-Serverless-NoSQL-for-the-NET-Developer" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/On-NET/Jeremy-Likness-CosmosDB-and-NET-Core", Tutorial = true },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Whats-New-in-Azure-Cosmos-DB", Comments = "Info on what's happening with CosmosDB!" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Cosmos-DB-Comprehensive-Overview", Tutorial = false },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Container-Instances-for-Multiplayer-Gaming-Theater", Comments = "Info on how to use containers with games" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Planning-and-building-games-using-the-full-power-of-VSTS-Agile-CI--CD-end-to-end-demo" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Internet-of-Things-Show/Azure-IoT-Toolkit-extension-for-Visual-Studio-Code" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Connect-Next-Week-Picking-the-Right-Azure-VM-Star-Wars-Video-Cards-Life-After-the-Uniform-and-m" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Blogs/Azure/Log-Alerts-for-Application-Insights-Preview" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/AI-Show/The-Simplest-Machine-Learning" }
            };

            foreach (var item in starterVideos)
            {
                DocumentDBRepository<Video>.CreateDocument(item, "Videos");
            }

            var Docs = new List<Doc>();

            Doc[] starterDocs =  {
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/", Tutorial = false, Comments = "Event Grid is very interesting! Look at this later"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-rest-api", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/scripts/app-service-cli-backup-onetime", Tutorial = true},
                new Doc(true) { stringUrl = "https://azure.microsoft.com/en-us/overview/serverless-computing/", Tutorial = false, Comments = "Intro to serverless computing"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview", Tutorial = false},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/query-cheat-sheet"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/20-days-of-tips", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/faq"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/manage-account"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-twitter-email"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-github-webhook-triggered-function", Tutorial = true, Comments = "GitHub webhook triggered Azure Functions"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/scripts/functions-cli-create-function-app-vsts-continuous", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/durable-functions-overview"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/resize-images-on-storage-blob-upload-event", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/concepts", Comments = "Basic concepts on event grid.", Tutorial = false},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/quickstarts/csharp", Comments = "Quickstarts to get started with Azure Cognitive Services", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-serverless-api", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnetcore-get-started", Tutorial = true}
            };


            foreach (var item in starterDocs)
            {
                DocumentDBRepository<Doc>.CreateDocument(item, "Docs");
            }
        }


        private static async Task CreateCollections()
        {
            var c = GetConfiguration();

            var videoCollection = await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(c["database"]), new DocumentCollection { Id = "Videos" });
            var docsCollectionn = await client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(c["database"]), new DocumentCollection { Id = "Docs" });
        }

        private static bool CheckDBExists()
        {
            var c = GetConfiguration();
            var dbId = c["database"] ?? "NotesDB";

            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

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

        public static async void CreateDocument(T item, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
            await client.CreateDocumentAsync((UriFactory.CreateDocumentCollectionUri(c["database"], collectionName)), item);
        }

        public static string Search(T item, string collectionName, string searchTerms, string searchText)
        {

            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            var terms = searchTerms.Split(' ');
            var secondClause = string.Format(" OR CONTAINS({0}.{1}, @{1})", collectionName, terms[1].ToLower());

            var q = new SqlQuerySpec
            {
                QueryText = string.Format("SELECT * FROM {0} WHERE CONTAINS({0}.{1}, @{1})", collectionName, terms[0].ToLower()),
                Parameters = new SqlParameterCollection()
                {
                    new SqlParameter("@" + terms[0].ToLower(), searchText)

                }
            };

            if (!string.IsNullOrEmpty(terms[1]))
            {
                q.Parameters.Add(new SqlParameter("@" + terms[1], searchText));
                q.QueryText += secondClause;
            }

            List<T> queryResult = (client.CreateDocumentQuery<T>(
                     UriFactory.CreateDocumentCollectionUri(c["database"], collectionName),
                     q,
                     queryOptions)).ToList();

            return JsonConvert.SerializeObject(queryResult);
        }

        public static async void DeleteDocument(string id, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

            var doc = GetDocument(id, collectionName);
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(c["database"], collectionName, id.ToString()), new RequestOptions { PartitionKey = new PartitionKey(doc.Name) });
        }

        public static async void EditDocument(Item item, string id, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

            Document doc = client.CreateDocumentQuery<Document>(UriFactory.CreateDocumentCollectionUri(c["database"], collectionName), new FeedOptions { EnableCrossPartitionQuery = true })
                                        .Where(r => r.Id == id.ToString())
                                        .AsEnumerable()
                                        .SingleOrDefault();

            doc.SetPropertyValue("comments", item.Comments);
            doc.SetPropertyValue("tutorial", item.Tutorial);


            Document updated = await client.ReplaceDocumentAsync(doc, new RequestOptions { PartitionKey = new PartitionKey(doc.GetPropertyValue<string>("name")) });
        }

        public static Item GetDocument(string id, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

            Item doc = client.CreateDocumentQuery<Item>(UriFactory.CreateDocumentCollectionUri(c["database"], collectionName), new FeedOptions { EnableCrossPartitionQuery = true })
                                        .Where(r => r.Id == id)
                                        .AsEnumerable()
                                        .SingleOrDefault();
            return doc;
        }

        public static Dictionary<string, string> GetConfiguration(string environmentName = null)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            environmentName = "ConnectionStrings";
            if (!String.IsNullOrWhiteSpace(environmentName))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            builder = builder.AddEnvironmentVariables();
            var config = builder.Build();

            var connStrings = new Dictionary<string, string>
            {
                { "database", config.GetConnectionString("database") },
                { "authkey", config.GetConnectionString("authkey") },
                { "endpoint", config.GetConnectionString("endpoint") }
            };

            return connStrings;
        }

        private async static Task CreateDB()
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

            try
            {
                Database database = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = "NotesDB" });
                c["database"] = database.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void CreateClient()
        {
            var c = GetConfiguration();
            client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
        }
    }
}
