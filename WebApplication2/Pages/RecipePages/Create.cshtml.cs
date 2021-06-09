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
            XElement recipe = XElement.Load(UploadFileName.OpenReadStream());
            var title = recipe.Descendants("title").FirstOrDefault()?.Value;
            var ingredients = recipe.Descendants("ingredient")
                .Select(el => TransformToIngredient);
            return RedirectToPage("./Index");
        }

        public Ingredient TransformToIngredient { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var ingredient in Ingredients)
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
                    Recipe = Recipe,
                    Ingredient = tmpIngredient,
                    Count = ingredient.Count
                };
                _context.Set<RecipeIngredient>().Add(recipeIngredient);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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

    public class IngredientWithCount
    {
        public string Name { get; set; }
        public uint Count { get; set; }
        public string Unit { get; set; }
    }
}