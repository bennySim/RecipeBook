using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2;

namespace WebApplication2.Pages.RecipePages
{
    public class DetailsModel : PageModel
    {
        private readonly RecipesContext _context;

        public DetailsModel(RecipesContext context)
        {
            _context = context;
        }

        public Recipe Recipe { get; set; }
        public List<IngredientWithCount> Ingredient { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Set<Recipe>().FirstOrDefaultAsync(m => m.Id == id);
            if (Recipe == null)
            {
                return NotFound();
            }

            Ingredient = _context.Set<RecipeIngredient>()
                .Where(ri => ri.RecipeId == id)
                .Select(ri => new IngredientWithCount(ri.Ingredient.Name, ri.Count, ri.Ingredient.Unit))
                .ToList();

            
            return Page();
        }

        public record IngredientWithCount(string Name, uint Count,string Unit);
    }
}
