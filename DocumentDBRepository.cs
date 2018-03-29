using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using contextual_notes.Pages;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace contextual_notes
{    public static class DocumentDBRepository<T> where T : class
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

        public static async void CreateDocument(T item, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);
            await client.CreateDocumentAsync((UriFactory.CreateDocumentCollectionUri(c["database"], collectionName)), item);
        }

        public static async void DeleteDocument(int id, string collectionName)
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

        public static Item GetDocument(int id, string collectionName)
        {
            var c = GetConfiguration();
            var client = new DocumentClient(new Uri(c["endpoint"]), c["authkey"]);

            Item doc = client.CreateDocumentQuery<Item>(UriFactory.CreateDocumentCollectionUri(c["database"], collectionName), new FeedOptions { EnableCrossPartitionQuery = true })
                                        .Where(r => r.Id == id.ToString())
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

            var connStrings = new Dictionary<string, string>();

            connStrings.Add("database", config.GetConnectionString("database"));
            connStrings.Add("authkey", config.GetConnectionString("authkey"));
            connStrings.Add("endpoint", config.GetConnectionString("endpoint"));

            return connStrings;
        }
    }
    }
