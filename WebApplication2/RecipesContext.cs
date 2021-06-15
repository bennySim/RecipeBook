using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2
{
    public sealed class RecipesContext : DbContext
    {
        public RecipesContext(DbContextOptions<RecipesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public RecipesContext()
        {
            Database.EnsureCreated();
        }
 
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=/home/simona/RiderProjects/WebApplication2/WebApplication2/RecipeBooks.db");
        
        public System.Data.Entity.DbSet<Recipe> Recipes { get; set; }
        public System.Data.Entity.DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public System.Data.Entity.DbSet<Ingredient> Ingredients { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new {ri.RecipeId, ri.IngredientId});

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);
               
            modelBuilder.Entity<Recipe>()
                .HasKey(o => o.Id);
            
            modelBuilder.Entity<Ingredient>()
                .HasKey(o => o.Id);
            
            modelBuilder.Entity<Recipe>()
                .Property(r => r.Category)
                .HasConversion<string>();
            
            base.OnModelCreating(modelBuilder);
            
        }
    }
}