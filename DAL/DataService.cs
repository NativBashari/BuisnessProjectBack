using Models.DataModels;

namespace DAL
{
    public class DataService
    {
        public List<Material> Materials;
        public List<Product> Products;
        public Buisness Buisness { get; set; }

        public DataService()
        {
            GenerateMaterials();
            GenerateProducts();
            GenearateBuisness();
        }

        private void GenearateBuisness()
        {
            this.Buisness = new Buisness() { Id = 1, Name = "Restaurant", Products = Products, ProductionSlots = 2 , ServiceStations = 4};
        }

        private void GenerateMaterials()
        {
            Materials = new List<Material>()
            {
                 new Material() { Id = 1, Name = "Bun", Amount = 100 },
                 new Material() { Id = 2, Name ="Onion", Amount=100},
                 new Material() { Id = 3, Name ="Lettuce", Amount=100},
                 new Material() { Id = 4, Name ="Tomato", Amount=100},
                 new Material() { Id = 5, Name ="Meat Ball", Amount=100},
                 new Material() { Id = 6, Name ="Ketchup", Amount=100}
            };
        }

        private void GenerateProducts()
        {
            this.Products = new List<Product>() {
            new Product() { Id = 1, Materials = this.Materials, Name = "Hamburger", DeliveryTime = 5, Priority= 0},
            new Product() { Id = 2, Materials = this.Materials, Name = "Salad", DeliveryTime = 5, Priority= 0},
            new Product() { Id = 3, Materials = this.Materials, Name = "Nodels", DeliveryTime = 5, Priority= 0}

             };
           
        
        
         
        }
    }
}