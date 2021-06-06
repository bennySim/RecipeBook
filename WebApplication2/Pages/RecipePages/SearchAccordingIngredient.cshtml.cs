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
        
        [BindProperty] public List<ChoosenIngredient> ChoosenIngredients { get;set; }
        public List<Ingredient> Ingredient { get; set; }

        public async Task OnGet()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            
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