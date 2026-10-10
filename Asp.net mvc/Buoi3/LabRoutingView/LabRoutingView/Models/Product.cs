namespace LabRoutingView.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool InStock { get; set; }

        public string Category => Price switch
        {
            < 1_000_000 => "Phổ thông",
            >= 1_000_000 and <= 10_000_000 => "Trung cấp",
            _ => "Cao cấp" //Default case for prices above 10,000,000
        };

    }
}