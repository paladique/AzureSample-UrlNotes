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
        public Item ItemToEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string collection)
        {
            //todo:replace testing hardcording
            //todo:how do you configure additional param in route?
           var item = DocumentDBRepository<object>.GetDocument(id,"Videos");
            ItemToEdit = new Item { Comments = item.Comments, Tutorial = item.Tutorial};
            
            return Page();
        }

        public void OnPostSave()
        {
            var x = ItemToEdit;
            DocumentDBRepository<Video>.EditDocument(Convert.ToInt32(x.Id), x.Id, "Videos");//x.i;
            //return null;
        }
    }
}
