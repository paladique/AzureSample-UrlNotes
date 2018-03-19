using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace contextual_notes.Pages
{
    public class IndexModel : PageModel
    {
        public List<string> Notes { get; set; }
        public string CollectionName { get; set; }
        [BindProperty]
        public Item NoteItem { get; set; }


        public void OnGet()
        {
            GetCollection().Wait();
        }

        public async Task GetCollection()
        {

            Notes = await DocumentDBRepository<object>.GetDBCollections();
        }

        public void OnGetDeleteAsync(int recordId, string pVal)
        {
            DocumentDBRepository<object>.DeleteDocument(recordId, pVal);
            //return null;
        }

        public void OnPostUpdate(int recordId, string pVal)
        {
            DocumentDBRepository<object>.EditDocument(recordId, pVal, CollectionName);
            //return null;
        }

        public void OnPostCreate()
        {
            var i = NoteItem;
            CollectionName = "Videos";
            switch (CollectionName)
            {
                case "Videos":
                    DocumentDBRepository<Video>.CreateDocument(new Video(NoteItem), CollectionName);
                    break;

                case "Docs":
                    DocumentDBRepository<Doc>.CreateDocument(null, CollectionName);
                    break;

                default:
                    break;
            }

        }

        public async Task<JsonResult> OnGetListAsync(string selectedCollection)
        {
            string json;

            switch (selectedCollection)
            {
                case "Videos":
                    CollectionName = "Videos";
                    json = await DocumentDBRepository<Video>.GetAllDocsAsync(selectedCollection);
                    break;

                case "Docs":
                    CollectionName = "Docs";
                    json = await DocumentDBRepository<Doc>.GetAllDocsAsync(selectedCollection);
                    break;

                default:
                    json = null;
                    break;
            }

            return new JsonResult(json);
        }
    }


    public class Item
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }
        [JsonProperty(PropertyName = "tutorial")]
        public bool Tutorial { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class Video : Item
    {
        internal Video(Item i)
        {
            Id = i.Id;
            Name = i.Name;    
            Url = i.Url;
            Comments = i.Comments;
            Tutorial = i.Tutorial;
        }

        [JsonConstructor]
        public Video()
        { }

        [JsonProperty(PropertyName = "length")]
        public int Length { get; set; }
    }

    public class Doc : Item
    {

    }

}


