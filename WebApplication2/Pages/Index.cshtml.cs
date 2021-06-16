using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class IndexModel : PageModel
    {
        private readonly RecipesContext _context;

        public IndexModel(RecipesContext context)
        {
            _context = context;
        }

        public IList<Recipe> Recipe { get; set; }

        [BindProperty(SupportsGet = true)] public string SearchString { get; set; }

        [BindProperty]
        public List<IngredientWithCount> IngredientsInRecipe { get; set; } =
            new() {new()};

        [BindProperty] public Category? Category { get; set; }

        public List<Ingredient> Ingredient { get; set; }

        public async Task OnGetAsync()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
            Recipe = await _context.Set<Recipe>().ToListAsync();
        }

        public async Task<PageResult> OnPost()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
            WriteMessage();
            await FindRecipesWithIngredients();
            return Page();
        }

        private async Task FindRecipesWithIngredients()
        {
            var recipes = await _context.Set<Recipe>().ToListAsync();

            if (!string.IsNullOrEmpty(SearchString))
            {
                SearchString = SearchString.ToLower();
                recipes = recipes.Where(r => r.Name.ToLower().Contains(SearchString)).ToList();
            }

            if (Category is not null)
            {
                recipes = recipes.Where(r => r.Category == Category).ToList();
            }

            if (IngredientsInRecipe.Count > 1 || IngredientsInRecipe[0].Count != 0)
            {
                var recipeIngredients = await _context.Set<RecipeIngredient>().ToListAsync();
                Recipe = IngredientsInRecipe.Where(r => r.Count > 0)
                    .Join(recipeIngredients, ri => ri.Id, ri => ri.IngredientId, (ri1, ri2) =>
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
                    .Where(g => Category is null || g.Recipe.Category == Category)
                    .Select(g => g.Recipe)
                    .ToList();
            }
            else
            {
                Recipe = recipes;
            }
        }

        private void WriteMessage()
        {
            var message = "Recipes with";
            var submessages = new List<string>();
            if (!string.IsNullOrEmpty(SearchString))
            {
                submessages.Add(" name containing " + SearchString);
            }

            if (Category is not null)
            {
                submessages.Add(" in category " + Category);
            }

            if (IngredientsInRecipe.Count > 1 || IngredientsInRecipe[0].Count != 0)
            {
                submessages.Add(" from ingredients " + IngredientsInRecipe
                    .Join(Ingredient, i => i.Id, i => i.Id, (ir, i) =>
                        new
                        {
                            i.Name,
                            i.Unit,
                            ir.Count
                        })
                    .Select(i =>" " + i.Count + " " + i.Unit + " of " + i.Name)
                    .Aggregate((r1, r2) => r1 + ", " + r2)
                );
            }

            message += string.Join(',', submessages) + '.';
            ViewData["Message"] = message;
        }
    }
}