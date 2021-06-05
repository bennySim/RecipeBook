using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2;

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

        private string ValidationResult = null;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var ingredient in Ingredients)
            {
                RecipeIngredient recipeIngredient = new RecipeIngredient()
                {
                    Recipe = Recipe,
                    Ingredient = new Ingredient() {Name = ingredient.Name, Unit = ingredient.Unit},
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