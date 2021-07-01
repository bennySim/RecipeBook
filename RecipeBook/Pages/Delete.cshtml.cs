using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.RecipePages
{
    public class DeleteModel : PageModel
    {
        private readonly DatabaseManipulation _databaseManipulation;

        public DeleteModel(RecipesContext context)
        {
            _databaseManipulation = new DatabaseManipulation(context);
        }

        [BindProperty]
        public Recipe Recipe { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _databaseManipulation.GetRecipeWithIdAsync(id);

            if (Recipe == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _databaseManipulation.RemoveRecipeWithIdAsync(id);

            return RedirectToPage("./Index");
        }

   
    }
}
