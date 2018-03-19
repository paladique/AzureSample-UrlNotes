using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace contextual_notes.Pages
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Item ItemToEdit { get; set; }
        [BindProperty]
        public string Collection { get; set; }

        public void OnGet(int id, string collection)
        {
           var item = DocumentDBRepository<object>.GetDocument(id, collection);
           ItemToEdit = new Item { Id = item.Id, Comments = item.Comments, Tutorial = item.Tutorial};
           Collection = collection;
        }

        public void OnPostSave()
        {
            DocumentDBRepository<Item>.EditDocument(ItemToEdit, ItemToEdit.Id,Collection);
        }
    }
}
