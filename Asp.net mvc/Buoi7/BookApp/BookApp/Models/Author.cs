using System.ComponentModel.DataAnnotations;

namespace BookApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        public List<Book> Books { get; set; } = new List<Book>();

        public List<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
