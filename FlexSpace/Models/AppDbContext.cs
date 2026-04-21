using FlexSpace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static FlexSpace.Models.Booking;


namespace FlexSpace.Models // 記得替換 Namespace
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BeautyService> BeautyServices { get; set; }
        public DbSet<OpeningHour> OpeningHours { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Beautician> Beauticians { get; set; }
        // ... 底下的 OnModelCreating 保留不變 ...
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🌟 重新設定 BeautyService 與 OpeningHours 的關聯
            modelBuilder.Entity<BeautyService>()
                .HasMany(b => b.OpeningHours)
                .WithOne(o => o.BeautyService)
                .HasForeignKey(o => o.BeautyServiceId);

            modelBuilder.Entity<BeautyService>()
                .HasMany(b => b.Bookings)
                .WithOne(bk => bk.BeautyService)
                .HasForeignKey(bk => bk.BeautyServiceId);

            modelBuilder.Entity<BeautyService>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);
        }
    }
}