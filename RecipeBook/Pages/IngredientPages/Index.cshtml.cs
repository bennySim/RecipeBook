using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication2;

namespace WebApplication2.Pages.IngredientPages
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication2.RecipesContext _context;

        public IndexModel(WebApplication2.RecipesContext context)
        {
            _context = context;
        }

        public IList<Ingredient> Ingredient { get;set; }

        public async Task OnGetAsync()
        {
            Ingredient = await _context.Set<Ingredient>().ToListAsync();
        }
    }
}
