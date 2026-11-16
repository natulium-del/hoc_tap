using Microsoft.EntityFrameworkCore;
using BookApp.Models;
namespace BookApp.Data
{
    public class BookDbContext(DbContextOptions<BookDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<PriceOffer> PriceOffers => Set<PriceOffer>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //1-1 PriceOffer là phía phụ thuộc
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Promotion)
                .WithOne(po => po.Book)
                .HasForeignKey<PriceOffer>(po => po.BookId);
            //N-N có payload
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .UsingEntity<BookAuthor>(
                    j => j.HasOne(ba => ba.Author).WithMany(a => a.BookAuthors).HasForeignKey(ba => ba.AuthorId),
                    j => j.HasOne(ba => ba.Book).WithMany(b => b.BookAuthors).HasForeignKey(ba => ba.BookId),
                    j =>
                    {
                        
                        j.HasKey(t => new { t.BookId, t.AuthorId });
                    }
                );
            // Bộ lọc toàn cục có tên (EF Core 10): ẩn sách đã xóa mềm khỏi MỌI truy vấn Book
            modelBuilder.Entity<Book>().HasQueryFilter("SoftDelete", b => !b.SoftDeleted);
            modelBuilder.Entity<Book>().HasIndex(b => b.PublishedOn);
        }
    }
}
