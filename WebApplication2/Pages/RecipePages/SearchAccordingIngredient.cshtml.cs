using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class SearchAccordingIngredient : PageModel
    {
        private readonly RecipesContext _context;

        public SearchAccordingIngredient(RecipesContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<RecipeIngredient> IngredientsInRecipe { get; set; }
        public List<Ingredient> Ingredient { get; set; }

        public async Task OnGet()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            
            for (int i = 0; i < IngredientsInRecipe.Count; i++)
            {
                Int32.TryParse(Request.Form["ingredient"+i].ToString(), out var id);
                IngredientsInRecipe[i].IngredientId = id;
            }
            
            return RedirectToPage("./Index");
        }

        public class ChoosenIngredient
        {
            public int Id { get; set; }
            public uint Count { get; set; }
            public bool Checked { get; set; }
        }
    }
}