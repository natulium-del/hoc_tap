using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;
using static ProjectManager.Models.Resource;

namespace ProjectManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

   
    public DbSet<Resource> Resources => Set<Resource>();    // truy vấn mọi loại nhân sự
    public DbSet<Developer> Developers => Set<Developer>(); // chỉ Developer
    public DbSet<Tester> Testers => Set<Tester>();          // chỉ Tester
    public DbSet<ResourceProfile> ResourceProfiles => Set<ResourceProfile>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Project> Projects => Set<Project>();
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === Bước 1: quan hệ 1 - N ===
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Customer)              // Phía Nhiều: 1 dự án có 1 khách hàng
            .WithMany(c => c.Projects)            // Phía Một: 1 khách hàng có nhiều dự án
            .HasForeignKey(p => p.CustomerId)     // Khóa ngoại
            .OnDelete(DeleteBehavior.Restrict);   // Chặn xóa Customer khi còn dự án
        // ===== BƯỚC 2: KẾ THỪA TPH =====
        modelBuilder.Entity<Resource>(e =>
        {
            // Unique index trên Email
            e.HasIndex(r => r.Email).IsUnique();

            // Cấu hình TPH discriminator
            e.HasDiscriminator<string>("ResourceType")   // Cột phân biệt loại nhân sự
                .HasValue<Developer>("Developer")        // Giá trị cho Developer
                .HasValue<Tester>("QA");                 // Giá trị cho Tester
            e.Property("ResourceType").HasMaxLength(8);
        });
        //===== BƯỚC 3: 1 – 1 =====
        modelBuilder.Entity<Resource>()
            .HasOne(r => r.Profile)
            .WithOne(p => p.Resource)
            .HasForeignKey<ResourceProfile>(p => p.ResourceId)
            .OnDelete(DeleteBehavior.Cascade); // xóa nhân sự => xóa hồ sơ
                                               // trong OnModelCreating – ===== BƯỚC 4: N – N =====
                                               // (a) Project – Tag: chỉ cần ràng buộc cho Tag, bảng nối theo quy ước
        modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();

        // (b) Project – Resource qua entity nối Assignment
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Resources)
            .WithMany(r => r.Projects)
            .UsingEntity<Assignment>(
                // phía phải: Assignment -> Resource
                r => r.HasOne(a => a.Resource).WithMany(x => x.Assignments).HasForeignKey(a => a.ResourceId),
                // phía trái: Assignment -> Project
                l => l.HasOne(a => a.Project).WithMany(x => x.Assignments).HasForeignKey(a => a.ProjectId),
                // cấu hình chính entity nối
                j =>
                {
                    j.ToTable("Assignments");
                    j.HasKey(a => new { a.ProjectId, a.ResourceId }); // khóa chính phức hợp
                });
    }
}