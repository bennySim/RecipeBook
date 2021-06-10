using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class EditModel : PageModel
    {
        private readonly RecipesContext _context;

        public EditModel(RecipesContext context)
        {
            _context = context;
        }

        [BindProperty] public Recipe Recipe { get; set; }

        [BindProperty] public List<IngredientWithCount> Ingredients { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Recipe = await _context.Set<Recipe>().Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(m => m.Id == id);
            Ingredients = await GetIngredientsOfRecipe(Recipe.RecipeIngredients);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }

        private async Task<List<IngredientWithCount>> GetIngredientsOfRecipe(
            ICollection<RecipeIngredient> ingredientsInRecipe)
        {
            var ingredients = await _context.Set<Ingredient>().AsNoTracking().ToListAsync();

            return ingredientsInRecipe
                .Join(ingredients, ri => ri.IngredientId, i => i.Id, (ri, i) =>
                    new IngredientWithCount() {Id = i.Id, Name = i.Name, Count = ri.Count, Unit = i.Unit})
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(Recipe.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var ingredientsInRecipe = await _context.Set<RecipeIngredient>()
                .AsNoTracking()
                .Where(ri => ri.RecipeId == Recipe.Id).ToListAsync();
            var ingredients = await GetIngredientsOfRecipe(ingredientsInRecipe);
            var changedIngredients = Ingredients.Where(i => !ingredients.Contains(i));
            changedIngredients.Where(i => OnlyCountIsDifferent(i, ingredients))
                .ToList()
                .ForEach(i => ingredientsInRecipe.FirstOrDefault(ir => ir.IngredientId == i.Id).Count = i.Count);
            changedIngredients.Where(i => !OnlyCountIsDifferent(i, ingredients))
                .ToList()
                .ForEach(async i => await UpdateIngredients(i));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
            }

            return RedirectToPage("./Index");
        }

        private async Task UpdateIngredients(IngredientWithCount ingredient)
        {
            if (ingredient.Id != 0)
            {
                RecipeIngredient recipeIngredient = new RecipeIngredient()
                    {RecipeId = Recipe.Id, IngredientId = ingredient.Id};
                _context.Set<RecipeIngredient>().Remove(recipeIngredient);
                await _context.SaveChangesAsync();
            }

            var ingredientInDatabase = _context.Set<Ingredient>()
                .AsNoTracking()
                .FirstOrDefault(i => i.Name == ingredient.Name && i.Unit == ingredient.Unit);
            RecipeIngredient newRecipeIngredient;
            if (ingredientInDatabase is not null)
            {
                newRecipeIngredient = new RecipeIngredient()
                    {Recipe = Recipe, Ingredient = ingredientInDatabase, Count = ingredient.Count};
            }
            else
            {
                var newIngredient = new Ingredient() {Name = ingredient.Name, Unit = ingredient.Unit};
                newRecipeIngredient = new RecipeIngredient()
                    {Recipe = Recipe, Ingredient = newIngredient, Count = ingredient.Count};
            }

            _context.Set<RecipeIngredient>().Add(newRecipeIngredient);
            await _context.SaveChangesAsync();
        }

        private bool OnlyCountIsDifferent(IngredientWithCount ingredient,
            List<IngredientWithCount> ingredientWithCounts)
        {
            return ingredientWithCounts.Any(i =>
                i.Name == ingredient.Name && i.Id == ingredient.Id && i.Unit == ingredient.Unit);
        }

        private bool RecipeExists(int id)
        {
            return _context.Set<Recipe>().Any(e => e.Id == id);
        }
    }
}