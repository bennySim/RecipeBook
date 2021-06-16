using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.RecipePages
{
    public class CreateModel : PageModel
    {
        private readonly RecipesContext _context;

        public CreateModel(RecipesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Recipe Recipe { get; set; }

        [BindProperty] public IFormFile UploadFileName { get; set; }
        [BindProperty] public List<IngredientWithCount> Ingredients { get; set; } = new() {new IngredientWithCount()};
        [BindProperty] public uint Count { get; set; }

        private string ValidationResult;

        public async Task<IActionResult> OnPostParseXML()
        {
            ParseRecipeFromXml(out var recipe, out var ingredients);
            await AddRecipeToDatabase(recipe, ingredients);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await AddRecipeToDatabase(Recipe, Ingredients);

            return RedirectToPage("./Index");
        }

        private void ParseRecipeFromXml(out Recipe recipe, out List<IngredientWithCount> ingredients)
        {
            var recipeEl = XElement.Load(UploadFileName.OpenReadStream());
            var name = recipeEl.Descendants("name").FirstOrDefault()?.Value;

            var cookingTimeStr = recipeEl.Descendants("cookingTime").FirstOrDefault()?.Value;
            Int32.TryParse(cookingTimeStr, out var cookingTime);

            var categoryStr = recipeEl.Descendants("category").FirstOrDefault()?.Value;
            Enum.TryParse(categoryStr, out Category category);

            var portionsStr = recipeEl.Descendants("portions").FirstOrDefault()?.Value;
            Int32.TryParse(portionsStr, out var portions);

            var instructions = recipeEl.Descendants("instructions").FirstOrDefault()?.Value;

            ingredients = recipeEl.Descendants("ingredient")
                .Select(TransformToIngredient)
                .ToList();
            
            recipe = new Recipe
            {
                Name = name,
                CookingTime = (uint) cookingTime,
                Category = category,
                Portions = (uint) portions,
                Instructions = instructions
            };
        }

        private IngredientWithCount TransformToIngredient(XElement el)
        {
            var name = el.Descendants("name").FirstOrDefault()?.Value;

            var countStr = el.Descendants("count").FirstOrDefault()?.Value;
            Int32.TryParse(countStr, out var count);

            var unit = el.Descendants("unit").FirstOrDefault()?.Value;

            return new IngredientWithCount {Name = name, Count = (uint) count, Unit = unit};
        }

        private async Task AddRecipeToDatabase(Recipe recipe, List<IngredientWithCount> ingredients)
        {
            ingredients = ingredients.Where(i => !string.IsNullOrEmpty(i.Name) 
                                                 && i.Count != 0 
                                                 && !string.IsNullOrEmpty(i.Unit)).ToList();
            foreach (var ingredient in ingredients)
            {
                var ingredientToAdd = _context.Set<Ingredient>()
                                          .FirstOrDefault(i => i.Name == ingredient.Name && i.Unit == ingredient.Unit)
                                      ?? new Ingredient {Name = ingredient.Name, Unit = ingredient.Unit};
                RecipeIngredient recipeIngredient = new RecipeIngredient
                {
                    Recipe = recipe,
                    Ingredient = ingredientToAdd,
                    Count = ingredient.Count
                };
                await _context.Set<RecipeIngredient>().AddAsync(recipeIngredient);
            }

            await _context.SaveChangesAsync();
        }

        void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            ValidationResult = args.Severity switch
            {
                XmlSeverityType.Error => $"Error: {args.Message}",
                XmlSeverityType.Warning => $"Warning {args.Message}",
                _ => ""
            };
        }
    }
}