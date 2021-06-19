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

        /// <summary>
        /// Add ingredient connected to recipe to database.
        /// If ingredient has empty name or unit or count is zero, nothing is added
        /// Ingredient is normalized, so that name and unit is in lower case
        /// </summary>
        /// <param name="ingredient">ingredient to add</param>
        /// <param name="recipe">recipe to which ingredient will be added</param>
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
                Ingredient = FindIngredient(ingredient),
                Count = ingredient.Count
            };

            await _context.Set<RecipeIngredient>().AddAsync(recipeIngredientToAdd);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Return Recipe according to id. RecipeIngredient table is included.
        /// </summary>
        /// <param name="id">id of recipe to find</param>
        public async Task<Recipe> GetRecipeWithIdAsync(int? id)
        {
            return await _context.Set<Recipe>()
                .Include(r => r.RecipeIngredients)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        
        /// <summary>
        /// Remove recipe according to id
        /// If recipe with such id does not exists, nothing happens
        /// </summary>
        /// <param name="id">id of recipe to remove</param>
        public async Task RemoveRecipeWithIdAsync(int? id)
        {
            var recipe = await _context.Set<Recipe>().FindAsync(id);

            if (recipe != null)
            {
                _context.Set<Recipe>().Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Remove ingredient for recipe
        /// </summary>
        /// <param name="recipeIngredient">recipe ingredient record to remove</param>
        public async Task RemoveRecipeIngredientAsync(RecipeIngredient recipeIngredient)
        {
            _context.Set<RecipeIngredient>().Remove(recipeIngredient);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Find ingredients for recipe with specified id including count
        /// </summary>
        /// <param name="id">id of ingredient to find</param>
        public List<IngredientWithCount> GetRecipeIngredientsWithCount(int? id)
        {
            return _context.Set<RecipeIngredient>()
                .Where(ri => ri.RecipeId == id)
                .Select(ri => new IngredientWithCount
                    {Id = ri.IngredientId, Name = ri.Ingredient.Name, Count = ri.Count, Unit = ri.Ingredient.Unit})
                .ToList();
        }

        ///<summary>
        /// Return all ingredients in database
        /// </summary>
        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Set<Ingredient>().ToListAsync();
        }

        /// <summary>
        /// Return all recipes in database
        /// </summary>
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            return await _context.Set<Recipe>().ToListAsync();
        }

         /// <summary>
         /// Return all RecipeIngredient in database
         /// </summary>
        public async Task<List<RecipeIngredient>> GetAllRecipeIngredientAsync()
        {
            return await _context.Set<RecipeIngredient>().ToListAsync();
        }

        /// <summary>
        /// Checks whether recipe with specified id exists in database 
        /// </summary>
        /// <param name="id">id of recipe to check</param>
        public bool RecipeExists(int id)
        {
            return _context.Set<Recipe>().Any(e => e.Id == id);
        }

        /// <summary>
        /// Change Recipe state in database
        /// </summary>
        /// <param name="recipe">recipe which state is changing</param>
        /// <param name="state">new state of recipe</param>
        public async Task ChangeRecipeStateAsync(Recipe recipe, EntityState state)
        {
            _context.Attach(recipe).State = state;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///Return all RecipeIngredient-s in recipe specified by id
        /// </summary>
        /// <param name="id">id of recipe</param>
        public async Task<List<RecipeIngredient>> GetRecipeIngredientInRecipeAsync(int id)
        {
            return await _context.Set<RecipeIngredient>()
                .AsNoTracking()
                .Where(ri => ri.RecipeId == id)
                .ToListAsync();
        }

        /// <summary>
        /// Update RecipeIngredient specified by id
        /// </summary>
        /// <param name="ingredient">ingredient which is updated</param>
        public void UpdateRecipeIngredientAsync(RecipeIngredient ingredient)
        {
            _context.Set<RecipeIngredient>().Update(ingredient);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get all ingredients in recipe specified by id
        /// </summary>
        /// <param name="id">id of recipe</param>
        public async Task<List<IngredientWithCount>> GetIngredientsInRecipeWithCount(int id)
        {
            var ingredientsInRecipe = await GetRecipeIngredientInRecipeAsync(id);
            var ingredients = await _context.Set<Ingredient>().AsNoTracking().ToListAsync();

            return ingredientsInRecipe
                .Join(ingredients, ri => ri.IngredientId, i => i.Id, (ri, i) =>
                    new IngredientWithCount
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Count = ri.Count,
                        Unit = i.Unit
                    })
                .ToList();
        }

        private Ingredient FindIngredient(IngredientWithCount ingredient)
        {
            return _context.Set<Ingredient>()
                       .FirstOrDefault(i => i.Name == ingredient.Name && i.Unit == ingredient.Unit)
                   ?? new Ingredient {Name = ingredient.Name, Unit = ingredient.Unit};
        }
    }
}