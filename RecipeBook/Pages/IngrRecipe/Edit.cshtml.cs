using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2;

namespace WebApplication2.Pages.IngrRecipe
{
    public class EditModel : PageModel
    {
        private readonly WebApplication2.RecipesContext _context;

        public EditModel(WebApplication2.RecipesContext context)
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
           ViewData["IngredientId"] = new SelectList(_context.Set<Ingredient>(), "Id", "Id");
           ViewData["RecipeId"] = new SelectList(_context.Set<Recipe>(), "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(RecipeIngredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeIngredientExists(RecipeIngredient.RecipeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RecipeIngredientExists(int id)
        {
            return _context.Set<RecipeIngredient>().Any(e => e.RecipeId == id);
        }
    }
}
