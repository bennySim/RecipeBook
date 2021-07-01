using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2
{
    public class Recipe
    {
        [Key] public int Id { get; set; }

        [MaxLength(64)] public String Name { get; set; }

        [Display(Name = "Cooking Time")]
        public uint CookingTime { get; set; }

        public uint Portions { get; set; }
        
        public Category Category { get; set; }

        [DataType(DataType.Text)] public string Instructions { get; set; }
 
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }

    public enum Category
    {
        Lunch,
        Breakfast,
        Drink,
        Dessert,
    }

    public class RecipeIngredient
    {
        public uint Count { get; set; }
        
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }

        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }

    public class Ingredient
    {
        [Key] public int Id { get; set; }

        [MaxLength(64)] public string Name { get; set; }

        [MaxLength(64)] public string Unit { get; set; }
        
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}