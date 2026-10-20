using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using System.Numerics;

namespace PharmaGo.DataAccess
{
    public class PharmacyGoDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Drug> Drugs { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Pharmacy> Pharmacys { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        public DbSet<StockRequestDetail> StockRequestDetails { get; set; }
        public DbSet<UnitMeasure> UnitMeasures { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Log> Logs { get; set; }

        public PharmacyGoDbContext(DbContextOptions<PharmacyGoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Drug>().Property(property => property.Price).HasPrecision(14, 2);
            modelBuilder.Entity<Purchase>().Property(property => property.TotalAmount).HasPrecision(14, 2);
            modelBuilder.Entity<PurchaseDetail>().Property(property => property.Price).HasPrecision(14, 2);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();
                entity.Property(b => b.BookerEmail).IsRequired();
                entity.Property(b => b.BookingDate).IsRequired();
                entity.Property(b => b.TotalAmount).IsRequired();
                entity.Property(b => b.Status).IsRequired();
                entity.Property(b => b.PublicKey).IsRequired();
                entity.HasMany(b => b.Details)
                    .WithOne(d => d.Booking)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookingDetail>(ent =>
            {
                ent.HasKey(bd => bd.Id);
                ent.Property(bd => bd.Code).IsRequired();
                ent.Property(bd => bd.Name).IsRequired();
                ent.Property(bd => bd.Quantity).IsRequired();
                ent.Property(bd => bd.Price).IsRequired();
                ent.Property(bd => bd.PharmacyId).IsRequired();
                ent.Property(bd => bd.PharmacyName).IsRequired();
            });


            modelBuilder.Entity<UnitMeasure>().Property(u => u.Name).HasConversion<string>();
            modelBuilder.Entity<Presentation>().Property(u => u.Name).HasConversion<string>();

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Timestamp).IsRequired();
                entity.Property(l => l.Level).IsRequired();
                entity.Property(l => l.Message).IsRequired();
            });

            base.OnModelCreating(modelBuilder);

        }

    }
}