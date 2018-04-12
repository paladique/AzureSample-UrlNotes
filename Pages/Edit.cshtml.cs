using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using contextual_notes.Models;


namespace contextual_notes.Pages
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Item ItemToEdit { get; set; }
        [BindProperty]
        public string Collection { get; set; }

        public void OnGet(string id, string collection)
        {
           var item = DocumentDBRepository.GetDocumentItem<Item>(id, collection);
           ItemToEdit = new Item {Name = item.Name, Id = item.Id, Notes = item.Notes, IsTutorial = item.IsTutorial};
           Collection = collection;
        }

        public IActionResult OnPostSave()
        {
            DocumentDBRepository.EditDocument(ItemToEdit,Collection);
            return Redirect("~/Index");
        }
    }
}
