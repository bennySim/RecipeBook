using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2;

namespace WebApplication2.Pages.IngrRecipe
{
    public class DeleteModel : PageModel
    {
        private readonly WebApplication2.RecipesContext _context;

        public DeleteModel(WebApplication2.RecipesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RecipeIngredient RecipeIngredient { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RecipeIngredient = await _context.Set<RecipeIngredient>()
                .Include(r => r.Ingredient)
                .Include(r => r.Recipe).FirstOrDefaultAsync(m => m.RecipeId == id);

            if (RecipeIngredient == null)
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

            RecipeIngredient = await _context.Set<RecipeIngredient>().FindAsync(id);

            if (RecipeIngredient != null)
            {
                _context.Set<RecipeIngredient>().Remove(RecipeIngredient);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
