using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.RecipePages
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseManipulation _databaseManipulation;
        public IndexModel(RecipesContext context)
        {
            _databaseManipulation = new DatabaseManipulation(context);
        }

        public IList<Recipe> Recipe { get; set; }

        [BindProperty(SupportsGet = true)] public string SearchString { get; set; }

        [BindProperty] public List<IngredientWithCount> IngredientsInRecipe { get; set; }

        [BindProperty] public Category? Category { get; set; }

        public List<Ingredient> Ingredient { get; set; }

        public async Task OnGetAsync()
        {
            Ingredient = await _databaseManipulation.GetAllIngredientsAsync();
            Recipe = await _databaseManipulation.GetAllRecipesAsync();
        }

       


        public async Task<PageResult> OnPost()
        {
            Ingredient = await _databaseManipulation.GetAllIngredientsAsync();
            WriteMessage();
            await FindRecipesWithIngredients();
            return Page();
        }

        private async Task FindRecipesWithIngredients()
        {
            Recipe = await _databaseManipulation.GetAllRecipesAsync();
            if (!string.IsNullOrEmpty(SearchString))
            {
                FilterAccordingKeyword();
            }

            if (Category is not null)
            {
                FilterAccordingCategory();
            }

            if (IngredientsInRecipe.Count > 1 || IngredientsInRecipe[0].Count != 0)
            {
                await FilterAccordingIngredients();
            }
        }

        private async Task FilterAccordingIngredients()
        {
            var recipeIngredients = await _databaseManipulation.GetAllRecipeIngredientAsync();
            Recipe = IngredientsInRecipe.Where(r => r.Count > 0)
                .Join(recipeIngredients, ri => ri.Id, ri => ri.IngredientId, (ri1, ri2) =>
                    new {Recipe = ri2, ri1.Count})
                .Where(r => r.Recipe.Count <= r.Count)
                .GroupBy(r => r.Recipe.RecipeId)
                .Join(Recipe, rg => rg.Key, r => r.Id, (rg, r) =>
                    new
                    {
                        Recipe = r,
                        Count = rg.Count()
                    })
                .Where(g => g.Recipe.RecipeIngredients.Count == g.Count)
                .Select(g => g.Recipe)
                .ToList();
        }

        private void FilterAccordingCategory()
        {
            Recipe = Recipe.Where(r => r.Category == Category).ToList();
        }

        private void FilterAccordingKeyword()
        {
            SearchString = SearchString.ToLower();
            Recipe = Recipe.Where(r => r.Name.ToLower().Contains(SearchString)).ToList();
        }

        private void WriteMessage()
        {
            const string messageStart = "Recipes with";
            var subMessages = new List<string>();
            if (!string.IsNullOrEmpty(SearchString))
            {
                subMessages.Add(" name containing " + SearchString);
            }

            if (Category is not null)
            {
                subMessages.Add(" in category " + Category);
            }

            if (IngredientsInRecipe.Count > 1 || IngredientsInRecipe[0].Count != 0)
            {
                subMessages.Add(AddMessageIngredients());
            }

            if (subMessages.Count != 0)
            {
                ViewData["Message"] = messageStart + string.Join(',', subMessages) + '.';
            }
        }

        private string AddMessageIngredients()
        {
            return " from ingredients " + IngredientsInRecipe
                .Join(Ingredient, i => i.Id, i => i.Id, (ir, i) =>
                    new
                    {
                        i.Name,
                        i.Unit,
                        ir.Count
                    })
                .Select(i => " " + i.Count + " " + i.Unit + " of " + i.Name)
                .Aggregate((r1, r2) => r1 + ", " + r2);
        }
    }
}