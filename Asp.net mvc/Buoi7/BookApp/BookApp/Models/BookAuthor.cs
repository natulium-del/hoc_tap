using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookApp.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        public byte Order { get; set; } // thứ tự tác giả

        public Book Book { get; set; } = null!;
        public Author Author { get; set; } = null!;
    }
}
