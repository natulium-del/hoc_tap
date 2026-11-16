using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookApp.Models
{
    public class PriceOffer
    {
        [Key]
        public int BookId { get; set; } // PK = FK

        [Precision(9, 2)]
        public decimal NewPrice { get; set; }

        [Required, StringLength(200)]
        public string PromotionalText { get; set; } = "";

        public Book Book { get; set; } = null!;
    }
}
