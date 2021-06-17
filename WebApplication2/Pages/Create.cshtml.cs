using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.RecipePages
{
    public class CreateModel : PageModel
    {
        private readonly DatabaseManipulation _databaseManipulation;

        public CreateModel(RecipesContext context)
        {
            _databaseManipulation = new DatabaseManipulation(context);
        }

        [BindProperty] public Recipe Recipe { get; set; }

        [BindProperty] public IFormFile UploadFileName { get; set; }

        [BindProperty] public List<IngredientWithCount> Ingredients { get; set; }

        private string _validationResult;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await AddRecipeToDatabase(Recipe, Ingredients);

            return RedirectToPage("./Index");
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostParseXML()
        {
            var isValid = ValidateFile(out var doc);
            if (!isValid)
            {
                Recipe = new Recipe();
                Ingredients = new List<IngredientWithCount> {new()};
                return Page();
            }

            ParseRecipeFromXml(doc, out var recipe, out var ingredients);
            await AddRecipeToDatabase(recipe, ingredients);
            return RedirectToPage("./Index");
        }

        private bool ValidateFile(out XDocument doc)
        {
            var schema = new XmlSchemaSet();
            schema.Add("", "./Pages/Shared/recipeScheme.xsd");
            var reader = XmlReader.Create(UploadFileName.OpenReadStream());

            doc = XDocument.Load(reader);
            doc.Validate(schema, ValidationEventHandler);

            if (!string.IsNullOrEmpty(_validationResult))
            {
                ViewData["Validation"] = _validationResult;
                return false;
            }

            return true;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            _validationResult = args.Severity switch
            {
                XmlSeverityType.Error => $"Error: {args.Message}",
                XmlSeverityType.Warning => $"Warning {args.Message}",
                _ => ""
            };
        }

        private void ParseRecipeFromXml(XDocument doc, out Recipe recipe, out List<IngredientWithCount> ingredients)
        {
            var recipeEl = doc.Root;
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
            foreach (var i in ingredients)
            {
                await _databaseManipulation.AddIngredientAsync(i, recipe);
            }
        }
    }
}
