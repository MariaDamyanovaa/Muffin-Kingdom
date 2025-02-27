namespace MVC_Muffin_Kingdon.Data
{
    public class Order
    {
        public int Id { get; set; } //fk m ->1
        public string UserId { get; set; }
        public User Users { get; set; }
        public int ProductId { get; set; }
        public Product Products { get; set; }
        public int Quantity { get; set; }
        public DateTime DateReg { get; set; }
        public string Description { get; set; }

    }
}