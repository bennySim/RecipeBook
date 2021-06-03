using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*var options = new DbContextOptionsBuilder<RecipesContext>()
                .UseInMemoryDatabase("RecipeBook")
                .Options;*/
        /*using (var ctx = new RecipesContext())//options))
        {
            ctx.Database.EnsureCreated();
            var stud = new Recipe() {Name = "Babovka"};

            ctx.Recipes.Add(stud);
            ctx.SaveChanges();

            Console.WriteLine("Querying for a blog");
            foreach (var recipe in ctx.Recipes)
            {
                Console.WriteLine($"{recipe.Name}");
            }
        }*/
        //using var context = DbFactory.CreateDbContext(); 

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}