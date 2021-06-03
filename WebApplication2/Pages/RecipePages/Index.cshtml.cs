using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Pages.RecipePages
{
    public class IndexModel : PageModel
    {
        private readonly RecipesContext _context;

        public IndexModel(RecipesContext context)
        {
            _context = context;
        }

        public IList<Recipe> Recipe { get;set; }
        
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        
        public async Task OnGetAsync()
        {
            var recipes = _context.Set<Recipe>().Select(m => m);
            if (!string.IsNullOrEmpty(SearchString))
            {
                recipes = recipes.Where(r => r.Name.Contains(SearchString));
            }
            Recipe = await recipes.ToListAsync();
        }
    }
}
