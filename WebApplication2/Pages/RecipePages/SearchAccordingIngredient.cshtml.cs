using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class SearchAccordingIngredient : PageModel
    {
        private readonly RecipesContext _context;

        public SearchAccordingIngredient(RecipesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<IngredientInRecipe> IngredientsInRecipe { get; set; } =
            new() {new()};

        public List<Ingredient> Ingredient { get; set; }
        public List<Recipe> Recipe { get; set; } = new();

        public async Task OnGet()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
        }

        public async Task OnPost()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
            WriteMessage();
            await FindRecipesWithIngredients();
        }

        private async Task FindRecipesWithIngredients()
        {
            var recipeIngredients = await _context.Set<RecipeIngredient>().ToListAsync();
            var recipes = await _context.Set<Recipe>().ToListAsync();

            Recipe = IngredientsInRecipe.Where(r => r.Count > 0)
                .Join(recipeIngredients, ri => ri.IngredientId, ri => ri.IngredientId, (ri1, ri2) =>
                    new {Recipe = ri2, ri1.Count})
                .Where(r => r.Recipe.Count <= r.Count)
                .GroupBy(r => r.Recipe.RecipeId)
                .Join(recipes, rg => rg.Key, r => r.Id, (rg, r) =>
                    new
                    {
                        Recipe = r,
                        Count = rg.Count()
                    })
                .Where(g => g.Recipe.RecipeIngredients.Count == g.Count)
                .Select(g => g.Recipe)
                .ToList();
        }

        private void WriteMessage()
        {
            ViewData["Message"] = "Recipes with " + IngredientsInRecipe
                .Join(Ingredient, i => i.IngredientId, i => i.Id, (ir, i) =>
                    new
                    {
                        i.Name,
                        i.Unit,
                        ir.Count
                    })
                .Select(i => i.Count + " " + i.Unit + " of " + i.Name)
                .Aggregate((r1, r2) => r1 + ", " + r2);
        }
    }

    public class IngredientInRecipe
    {
        public int IngredientId { get; set; }
        public uint Count { get; set; }
    }
}