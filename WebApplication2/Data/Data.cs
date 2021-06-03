using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2
{
    public class Recipe
    {
        [Key] public int Id { get; set; }

        //[Required]
        //     [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [MaxLength(64)] public String Name { get; set; }

        [Display(Name = "Cooking Time")]
        public uint CookingTime { get; set; }

        public uint Portions { get; set; }

        [DataType(DataType.Text)] public string Instructions { get; set; }
 
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
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

/*
namespace WebApplication2
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //     [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [MaxLength(64)] 
        public String Name { get; set; }

        public uint CookingTime { get; set; }

        public uint Portions { get; set; }
        
        [DataType(DataType.Text)]
        public string Instructions { get; set; }
        
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }

    public class RecipeIngredient
    {
        [Key] 
        public int RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; }

        [Key]
        public int IngredientId { get; set; }

        [ForeignKey(nameof(IngredientId))]
        public virtual Ingredient Ingredient { get; set; }

        public uint Count { get; set; }
    }

    public class Ingredient
    {
        [Key] 
        public int Id { get; set; }

        [MaxLength(64)] 
        public String Name { get; set; }
        
        [MaxLength(64)] 
        public uint Unit { get; set; }
        
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
*/