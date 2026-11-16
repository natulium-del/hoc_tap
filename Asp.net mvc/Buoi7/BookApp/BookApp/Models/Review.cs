using System.ComponentModel.DataAnnotations;

namespace BookApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string VoterName { get; set; } = "";

        [Range(1, 5)]
        public int NumStars { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public int BookId { get; set; }          // FK

        public Book Book { get; set; } = null!;  // navigation nhiều-1
    }
}
