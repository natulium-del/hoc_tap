using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;
namespace QuanLySinhVien.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Khoa> Khoas => Set<Khoa>();
        public DbSet<SinhVien> SinhViens => Set<SinhVien>();
        public DbSet<MonHoc> MonHocs => Set<MonHoc>();
        public DbSet<DangKyHoc> DangKyHocs => Set<DangKyHoc>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Khoa>(e =>
            {
                e.ToTable("Khoa");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.MaKhoa).IsUnique(); }
            );


            modelBuilder.Entity<SinhVien>(e =>
            {
                e.ToTable("SinhVien");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Lop); // tăng tốc lọc theo lớp
                e.Property(x => x.DiemTrungBinh).HasPrecision(4, 2);
                e.HasOne(x => x.Khoa) // quan hệ 1 - N
                    .WithMany(k => k.SinhViens)
                    .HasForeignKey(x => x.KhoaId)
                    .OnDelete(DeleteBehavior.Restrict); // chặn xóa Khoa còn sinh viên

            });
            modelBuilder.Entity<MonHoc>(e =>
            {
                e.ToTable("MonHoc");
                e.HasKey(x => x.Id);
            });
            modelBuilder.Entity<DangKyHoc>(e =>
            {
                e.ToTable("DangKyHoc");

                // Khóa chính tổ hợp - một sinh viên chỉ đăng ký một môn đúng một lần
                e.HasKey(x => new { x.SinhVienId, x.MonHocId });

                e.HasOne(x => x.SinhVien)
                 .WithMany(s => s.DangKyHocs)
                 .HasForeignKey(x => x.SinhVienId)
                 .OnDelete(DeleteBehavior.Cascade);      // xóa SV thì xóa luôn đăng ký

                e.HasOne(x => x.MonHoc)
                 .WithMany(m => m.DangKyHocs)
                 .HasForeignKey(x => x.MonHocId)
                 .OnDelete(DeleteBehavior.Restrict);     // chặn xóa môn còn người đăng ký

                e.Property(x => x.Diem).HasPrecision(4, 2);
            });
            
            // Cách viết cho EF Core 8 / 9
            modelBuilder.Entity<SinhVien>().HasQueryFilter(x => !x.DaXoa);

            // Cách viết cho EF Core 10 - bộ lọc có TÊN, tắt riêng được
            // modelBuilder.Entity<SinhVien>().HasQueryFilter("SoftDelete", x => !x.DaXoa);
        }
    }
}
