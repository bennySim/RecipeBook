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
        private readonly WebApplication2.RecipesContext _context;

        public CreateModel(WebApplication2.RecipesContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Recipe Recipe { get; set; }

        [BindProperty] public IFormFile UploadFileName { get; set; }
        [BindProperty] public List<Ingredient> Ingredients { get; set; } = new() {new Ingredient()};
        [BindProperty] public uint Count { get; set; }

        private string ValidationResult = null;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            // XmlReaderSettings settings = new XmlReaderSettings();
            //settings.Schemas.Add("http://www.formatdata.com/recipeml", "recipeml.dtd");
            /*//settings.ValidationEventHandler += new ValidationEventHandl
            settings.ValidationType = ValidationType.DTD;
            settings.DtdProcessing = DtdProcessing.Parse;
           // XmlReader reader = XmlReader.Create(UploadFileName.Name, settings);
            XmlDocument document = new XmlDocument();
           // document.Load(reader);
           // document.Validate(ValidationEventHandler);
            var titles = document.GetElementsByTagName("title");
            StringBuilder _builder = new StringBuilder();
            /*while (reader.Read())
            {
            }#1#

            if (_builder.ToString() == String.Empty)
                Console.Write("DTD Validation completed successfully.");
            else
                Console.Write("DTD Validation Failed. <br>" + _builder.ToString());*/
//            reader.Close();


            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var ingredient in Ingredients)
            {
                RecipeIngredient recipeIngredient = new RecipeIngredient()
                {
                    Recipe = Recipe,
                    Ingredient = ingredient,
                    Count = Count
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
}