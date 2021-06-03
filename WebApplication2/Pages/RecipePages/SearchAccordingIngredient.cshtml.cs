using System.Collections.Generic;
using System.Linq;
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
        
        public List<ChoosenIngredient> Ingredient { get;set; }

        public async Task OnGet()
        {
            var ingredients = _context.Set<Ingredient>()
                .Select(i => new ChoosenIngredient(i.Name, 0, false));
            Ingredient = await ingredients.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            return RedirectToPage("./Index");
        }

        public record ChoosenIngredient(string Name, uint Count, bool Checked);
    }
}