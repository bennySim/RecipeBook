using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class EditModel : PageModel
    {
        private readonly DatabaseManipulation _databaseManipulation;

        public EditModel(RecipesContext context)
        {
            _databaseManipulation = new DatabaseManipulation(context);
        }

        [BindProperty] public Recipe Recipe { get; set; }

        [BindProperty] public List<IngredientWithCount> Ingredients { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _databaseManipulation.GetRecipeWithIdAsync(id);

            Ingredients = await _databaseManipulation.GetRecipeIngredientsWithCount(id);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _databaseManipulation.ChangeRecipeStateAsync(Recipe, EntityState.Modified);
 
            var ingredientsInRecipe = await _databaseManipulation.GetRecipeIngredientInRecipeAsync(Recipe.Id);
            var changedIngredients = await GetChangedIngredients();

            if (changedIngredients.ContainsKey("CountIsDifferent"))
            {
                changedIngredients["CountIsDifferent"]
                    .ForEach(i => UpdateCountInIngredient(ingredientsInRecipe, i));
            }

            if (changedIngredients.ContainsKey("NotOnlyCountIsDifferent"))
            {
                changedIngredients["NotOnlyCountIsDifferent"]
                    .ForEach(async i => await UpdateIngredients(i));
            }
            return RedirectToPage("./Index");
        }

        private async Task<ImmutableDictionary<string, List<IngredientWithCount>>> GetChangedIngredients()
        {
            var ingredients = await _databaseManipulation.GetIngredientsInRecipeWithCount(Recipe.Id);
            return Ingredients
                .Where(i => i.Count != 0)
                .Where(i => !ingredients.Contains(i))
                .GroupBy(i => IsOnlyCountDifferent(i, ingredients))
                .ToImmutableDictionary(g => g.Key ? "CountIsDifferent" : "NotOnlyCountIsDifferent", g => g.ToList());
        }

        private void UpdateCountInIngredient(List<RecipeIngredient> ingredientsInRecipe,
            IngredientWithCount changedIngredient)
        {
            var ingredient = ingredientsInRecipe.FirstOrDefault(i => i.IngredientId == changedIngredient.Id);
            ingredient.Count = changedIngredient.Count;
            _databaseManipulation.UpdateRecipeIngredientAsync(ingredient);
        }

        private async Task UpdateIngredients(IngredientWithCount ingredient)
        {
            if (ingredient.Id != 0)
            {
                RecipeIngredient recipeIngredient = new RecipeIngredient {RecipeId = Recipe.Id, IngredientId = ingredient.Id};
                await _databaseManipulation.RemoveRecipeIngredientAsync(recipeIngredient);
            }

            await _databaseManipulation.AddIngredientAsync(ingredient, Recipe);
        }

        private static bool IsOnlyCountDifferent(IngredientWithCount ingredient,
            List<IngredientWithCount> ingredientWithCounts)
        {
            return ingredientWithCounts.Any(i =>
                i.Name == ingredient.Name && i.Id == ingredient.Id && i.Unit == ingredient.Unit);
        }

        
    }
}