using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;
namespace QuanLySinhVien.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        //Mỗi DbSet tương ứng một bảng 
        public DbSet<Khoa> Khoas => Set<Khoa>();
        public DbSet<SinhVien> SinhViens => Set<SinhVien>();
        public DbSet<MonHoc> MonHocs => Set<MonHoc>();
        public DbSet<DangKyHoc> DangKyHocs => Set<DangKyHoc>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // gọi phương thức gốc để thiết lập các cấu hình mặc định

            //Khoa
            modelBuilder.Entity<Khoa>(e =>
            {
                e.ToTable("Khoa");
                e.HasKey(k => k.Id);
                e.HasIndex(k => k.MaKhoa).IsUnique();
            });

            //SinhVien
            modelBuilder.Entity<SinhVien>(e =>
            {
                e.ToTable("SinhVien");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Lop);
                e.HasIndex(x => x.Email).IsUnique();

                e.HasOne(x => x.Khoa) //Quan hệ 1 - N với Khoa
                 .WithMany(k => k.SinhViens)
                 .HasForeignKey(x => x.KhoaId)
                 .OnDelete(DeleteBehavior.Restrict) //chặn xóa Khoa còn sinh viên

                e.HasQueryFilter(x => !x.DaXoa);
            });

            //MonHoc + Hash danh mục cố định
            modelBuilder.Entity<MonHoc>(e =>
             {
                e.ToTable("MonHoc");
                e.HasKey(x => x.Id);
                e.HasData(
                    new MonHoc { Id = 1, TenMon = "Toán Cao Cấp", SoTinChi = 3 },
                    new MonHoc { Id = 2, TenMon = "Vật Lý Đại Cương", SoTinChi = 4 },
                    new MonHoc { Id = 3, TenMon = "Hóa Học", SoTinChi = 3 },
                    new MonHoc { Id = 4, TenMon = "Cơ Sở Dữ Liệu", SoTinChi = 3 });
             });
            //DangKyHoc bảng nối N-N có dữ liệu
            modelBuilder.Entity<DangKyHoc>(e =>
            {
                e.ToTable("DangKyHoc");
                e.HasKey(x => new { x.SinhVienId, x.MonHocId }); // Khóa chính gồm 2 cột

                e.HasOne(x => x.SinhVien)
                 .WithMany(sv => sv.DangKyHocs)
                 .HasForeignKey(x => x.SinhVienId)
                 .OnDelete(DeleteBehavior.Cascade); //Xóa sv => xóa luôn đăng ký học

                e.HasOne(x => x.MonHoc)
                 .WithMany(mh => mh.DangKyHocs)
                 .HasForeignKey(x => x.MonHocId)
                 .OnDelete(DeleteBehavior.Restrict); //Chặn xóa môn học nếu còn sinh viên đăng ký
                e.HasQueryFilter(x => !x.SinhVien.DaXoa);
            });
        }
    }
}
