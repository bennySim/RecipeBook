using System.IO;

namespace WebApplication2
{
    public static class Config
    {
        public static string SchemaPath { get; } = "./Pages/Shared/recipeScheme.xsd";
        public static string DatabasePath { get; } = Path.Combine(Directory.GetCurrentDirectory(), "RecipeBooks.db");
    }
}