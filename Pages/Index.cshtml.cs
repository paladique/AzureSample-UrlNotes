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
        public List<string> Collections { get; set; }
        [BindProperty]
        public string CollectionName { get; set; }
        [BindProperty]
        public Item NoteItem { get; set; }


        public void OnGet()
        {
            GetCollection().Wait();
        }

        public async Task GetCollection()
        {
            Collections = await DocumentDBRepository<object>.GetDBCollections();
        }

        public IActionResult OnGetDeleteAsync(int recordId, string collection)
        {
            DocumentDBRepository<object>.DeleteDocument(recordId, collection);
            GetCollection().Wait();

            return Page();
        }

        public IActionResult OnPostCreate()
        {
            NoteItem.Name = Utils.Unfurl(NoteItem.Url);

            switch (CollectionName)
            {
                case "Videos":
                    DocumentDBRepository<Video>.CreateDocument(new Video(NoteItem), CollectionName);
                    break;

                case "Docs":
                    DocumentDBRepository<Doc>.CreateDocument(new Doc(NoteItem), CollectionName);
                    break;

                default:
                    break;
            }

            return Page();
        }

        public async Task<JsonResult> OnGetListAsync(string selectedCollection)
        {
            string json;

            switch (selectedCollection)
            {
                case "Videos":
                    json = await DocumentDBRepository<Video>.GetAllDocsAsync(selectedCollection);
                    break;

                case "Docs":
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
        internal Doc(Item i)
        {
            Id = i.Id;
            Name = i.Name;
            Url = i.Url;
            Comments = i.Comments;
            Tutorial = i.Tutorial;
        }

        [JsonConstructor]
        public Doc()
        { }
    }

}


