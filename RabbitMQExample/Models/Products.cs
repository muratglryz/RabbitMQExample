using System.ComponentModel.DataAnnotations;

namespace RabbitMQExample.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int ProductStock { get; set; } = 0;
        public string? ProductImage {  get; set; }

    }
}
