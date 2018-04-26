using contextual_notes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace contextual_notes.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<string> Collections { get; set; }
        [BindProperty]
        public string CollectionName { get; set; } = "Videos";
        [BindProperty]
        public Item NoteItem { get; set; }


        public void OnGet()
        {
            GetCollection().Wait();
        }

        public async Task GetCollection()
        {
            Collections = await DocumentDBRepository.GetDBCollections();
        }

        public IActionResult OnGetDeleteAsync(string recordId, string collection)
        {
            DocumentDBRepository.DeleteDocument(recordId, collection);
            GetCollection().Wait();
            CollectionName = collection;

            return Page();
        }

        public IActionResult OnPostCreate()
        {

            switch (CollectionName)
            {

                case "Videos":
                    var videoItem = new Video(NoteItem);
                    DocumentDBRepository.CreateDocument(videoItem, CollectionName);
                    break;

                case "Docs":
                    var docItem = new Doc(NoteItem);
                    DocumentDBRepository.CreateDocument(docItem, CollectionName);
                    break;

                default:
                    break;
            }

            GetCollection().Wait();
            return Page();
        }

        [HttpPost]
        public IActionResult OnPostSearch(string selectedCollection, string searchTerms, string searchText)
        {
            string json = null;
            switch (selectedCollection)
            {

                case "Videos":
                    json = DocumentDBRepository.Search<Video>(selectedCollection, searchTerms, searchText);
                    break;

                case "Docs":
                    json = DocumentDBRepository.Search<Doc>(selectedCollection, searchTerms, searchText);
                    break;

                default:
                    json = null;
                    break;
            }

            return new JsonResult(json);
        }

        public async Task<JsonResult> OnGetListAsync(string selectedCollection)
        {
            string json;

            switch (selectedCollection)
            {
                case "Videos":
                    json = await DocumentDBRepository.GetAllDocs<Video>(selectedCollection);
                    break;

                case "Docs":
                    json = await DocumentDBRepository.GetAllDocs<Doc>(selectedCollection);
                    break;

                default:
                    json = null;
                    break;
            }

            return new JsonResult(json);
        }
    }

}


