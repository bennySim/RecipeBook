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
        public List<RecipeIngredient> IngredientsInRecipe { get; set; } =
            new() {new()};

        public List<RecipeIngredient> Ingredients { get; set; }

        public List<Ingredient> Ingredient { get; set; }
        public List<Recipe> Recipe { get; set; } = new();

        public async Task OnGet()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
        }

        public async Task OnPost()
        {
            var recipeIngredients = await _context.Set<RecipeIngredient>().ToListAsync();
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
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
    }
}