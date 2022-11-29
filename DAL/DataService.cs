﻿using Models.DataModels;

namespace DAL
{
    public class DataService
    {
        public List<Material>? Materials;
        public List<Product>? Products;
        public Buisness? Buisness { get; set; }

        public DataService()
        {
            GenerateMaterials();
            GenerateProducts();
            GenearateBuisness();
        }

        private void GenearateBuisness()
        {
            this.Buisness = new Buisness() { Id = 1, Name = "Restaurant", Products = Products, ProductionSlots = 1 , ServiceStations = 2, DelieveryTime = 10};
        }

        private void GenerateMaterials()
        {
            Materials = new List<Material>()
            {
                 new Material() { Id = 1, Name = "Bun", Amount = 100},
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
            new Product() { Id = 1, Materials = this.Materials, Name = "Hamburger", DeliveryTime = 2, Priority= 0, Image = "data:image/png;base64,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"},
            new Product() { Id = 2, Materials = this.Materials, Name = "Salad", DeliveryTime = 2, Priority= 0, Image = "data:image/png;base64,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"},
            new Product() { Id = 3, Materials = this.Materials, Name = "Nodels", DeliveryTime = 2, Priority= 0, Image="data:image/png;base64,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"}

             };
           
        
        
         
        }
    }
}