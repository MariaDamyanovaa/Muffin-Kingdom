namespace MVC_Muffin_Kingdon.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public Category Categories { get; set; }
        public bool Gluten { get; set; }
        public decimal Weight { get; set; }
        public string Description { get; set; }
        public string URLImage { get; set; }
        public decimal Price { get; set; }
        public DateTime DataReg { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
