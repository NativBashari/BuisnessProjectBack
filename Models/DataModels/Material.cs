namespace Models.DataModels
{
    public class Material
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ProccessTime { get; set; }
        public int Amount { get; set; } //In stock
    }
}