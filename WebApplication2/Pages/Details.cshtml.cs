using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.RecipePages
{
    public class DetailsModel : PageModel
    {
        private readonly DatabaseManipulation _databaseManipulation;

        public DetailsModel(RecipesContext context)
        {
            _databaseManipulation = new DatabaseManipulation(context);
        }

        public Recipe Recipe { get; set; }
        public List<IngredientWithCount> Ingredient { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await FillRecipeANdIngredients(id);
            if (Recipe == null)
            {
                return NotFound();
            }


            return Page();
        }

        private async Task FillRecipeANdIngredients(int? id)
        {
            Recipe = await _databaseManipulation.GetRecipeWithIdAsync(id);
            Ingredient = _databaseManipulation.GetRecipeIngredientsWithCount(id);
        }

     

        public async Task<FileResult> OnGetSaveXml(int id)
        {
            await FillRecipeANdIngredients(id);
            var document = new XDocument(
                new XDeclaration("0.1", "utf-8", "yes"),
                new XElement("recipe",
                    new XElement("name", Recipe.Name),
                    new XElement("cookingTime", Recipe.CookingTime),
                    new XElement("portions", Recipe.Portions),
                    new XElement("ingredients", Ingredient.Select(CreateXmlIngredient)),
                    new XElement("instructions", Recipe.Instructions))
            );
            return File(Encoding.UTF8.GetBytes(document.ToString()), "application/xml", Recipe.Name + ".xml");
        }

        private XElement CreateXmlIngredient(IngredientWithCount ingredient)
        {
            return new("ingredient",
                new XElement("name", ingredient.Name),
                new XElement("count", ingredient.Count),
                new XElement("unit", ingredient.Unit));
        }
    }
}