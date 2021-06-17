using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2
{
    public class DatabaseManipulation
    {
        private readonly RecipesContext _context;

        public DatabaseManipulation(RecipesContext context)
        {
            _context = context;
        }

        public async Task AddIngredientAsync(IngredientWithCount ingredient, Recipe recipe)
        {
            if (string.IsNullOrEmpty(ingredient.Name) || string.IsNullOrEmpty(ingredient.Unit) || ingredient.Count == 0)
            {
                return;
            }

            ingredient.Normalize();

            var recipeIngredientToAdd = new RecipeIngredient
            {
                Recipe = recipe,
                Ingredient = FindIngredient(ingredient, _context),
                Count = ingredient.Count
            };

            await _context.Set<RecipeIngredient>().AddAsync(recipeIngredientToAdd);
            await _context.SaveChangesAsync();
        }

        public async Task<Recipe> GetRecipeWithIdAsync(int? id)
        {
            return await _context.Set<Recipe>()
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        private Ingredient FindIngredient(IngredientWithCount ingredient, RecipesContext context)
        {
            return context.Set<Ingredient>()
                       .FirstOrDefault(i => i.Name == ingredient.Name && i.Unit == ingredient.Unit)
                   ?? new Ingredient {Name = ingredient.Name, Unit = ingredient.Unit};
        }

        public async Task RemoveRecipeWithId(int? id)
        {
            var recipe = await _context.Set<Recipe>().FindAsync(id);

            if (recipe != null)
            {
                _context.Set<Recipe>().Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRecipeIngredientAsync(RecipeIngredient recipeIngredient)
        {
            _context.Set<RecipeIngredient>().Remove(recipeIngredient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IngredientWithCount>> GetRecipeIngredientsWithCount(int? id)
        {
            return _context.Set<RecipeIngredient>()
                .Where(ri => ri.RecipeId == id)
                .Select(ri => new IngredientWithCount
                    {Name = ri.Ingredient.Name, Count = ri.Count, Unit = ri.Ingredient.Unit})
                .ToList();
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Set<Ingredient>().ToListAsync();
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Set<Recipe>().ToListAsync();
        }

        public async Task<List<RecipeIngredient>> GetAllRecipeIngredientAsync()
        {
            return await _context.Set<RecipeIngredient>().ToListAsync();
        }

        public async Task<List<Ingredient>> GetAllIngredientsAsNoTrackingAsync()
        {
            return await _context.Set<Ingredient>().AsNoTracking().ToListAsync();
        }

        public bool RecipeExists(int id)
        {
            return _context.Set<Recipe>().Any(e => e.Id == id);
        }
    }
}