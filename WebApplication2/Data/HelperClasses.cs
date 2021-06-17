namespace WebApplication2
{
    public class IngredientWithCount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public uint Count { get; set; }
        public string Unit { get; set; }

        public override bool Equals(object? obj)
        {
            var ingredient = obj as IngredientWithCount;
            if (ingredient == null)
            {
                return false;
            }

            return Id == ingredient.Id && Name == ingredient.Name && Count == ingredient.Count && Unit == ingredient.Unit;
        }

        public void Normalize()
        {
            Name = Name.ToLower();
            Unit = Unit.ToLower();
        }
    }
}