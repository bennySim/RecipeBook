using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2;

namespace WebApplication2.Pages.IngrRecipe
{
    public class CreateModel : PageModel
    {
        private readonly WebApplication2.RecipesContext _context;

        public CreateModel(WebApplication2.RecipesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["IngredientId"] = new SelectList(_context.Set<Ingredient>(), "Id", "Id");
        ViewData["RecipeId"] = new SelectList(_context.Set<Recipe>(), "Id", "Id");
            return Page();
        }

        [BindProperty]
        public RecipeIngredient RecipeIngredient { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Set<RecipeIngredient>().Add(RecipeIngredient);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
