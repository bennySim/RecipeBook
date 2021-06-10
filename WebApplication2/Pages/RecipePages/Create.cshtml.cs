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
            XElement recipeEl = XElement.Load(UploadFileName.OpenReadStream());
            var name = recipeEl.Descendants("name").FirstOrDefault()?.Value;

            var cookingTimeStr = recipeEl.Descendants("cookingTime").FirstOrDefault()?.Value;
            Int32.TryParse(cookingTimeStr, out var cookingTime);

            var portionsStr = recipeEl.Descendants("portions").FirstOrDefault()?.Value;
            Int32.TryParse(portionsStr, out var portions);

            var instructions = recipeEl.Descendants("instructions").FirstOrDefault()?.Value;

            ingredients = recipeEl.Descendants("ingredient")
                .Select(TransformToIngredient)
                .ToList();
            recipe = new Recipe
            {
                Name = name, CookingTime = (uint) cookingTime, Portions = (uint) portions,
                Instructions = instructions
            };
        }

        private IngredientWithCount TransformToIngredient(XElement el)
        {
            var name = el.Descendants("name").FirstOrDefault()?.Value;

            var countStr = el.Descendants("count").FirstOrDefault()?.Value;
            Int32.TryParse(countStr, out var count);

            var unit = el.Descendants("unit").FirstOrDefault()?.Value;

            return new IngredientWithCount() {Name = name, Count = (uint) count, Unit = unit};
        }

        private async Task AddRecipeToDatabase(Recipe recipe, List<IngredientWithCount> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                var tmpIngredient = new Ingredient() {Name = ingredient.Name, Unit = ingredient.Unit};
                var inTable = _context.Set<Ingredient>()
                    .Where(i => i.Name == ingredient.Name && i.Unit == ingredient.Unit);
                if (inTable.Count() != 0)
                {
                    tmpIngredient = inTable.First();
                }

                RecipeIngredient recipeIngredient = new RecipeIngredient()
                {
                    Recipe = recipe,
                    Ingredient = tmpIngredient,
                    Count = ingredient.Count
                };
                _context.Set<RecipeIngredient>().Add(recipeIngredient);
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