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
            if (obj == null)
            {
                return false;
            }

            IngredientWithCount ingedient = obj as IngredientWithCount;
            if (ingedient == null)
            {
                return false;
            }

            return Id == ingedient.Id && Name == ingedient.Name && Count == ingedient.Count && Unit == ingedient.Unit;
        }
    }
}