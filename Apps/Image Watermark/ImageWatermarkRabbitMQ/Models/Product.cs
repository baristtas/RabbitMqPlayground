using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageWatermarkRabbitMQ.Models
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        [StringLength(100)]
        public string name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal price { get; set; }
        [Range(1,100)]
        public int stock { get; set; }
        [StringLength(100)]
        public string ImageName { get; set; }

    }
}
