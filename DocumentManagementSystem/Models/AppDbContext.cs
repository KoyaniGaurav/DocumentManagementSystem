using Microsoft.EntityFrameworkCore;

namespace DocumentManagementSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<DocumentShare> DocumentShares { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentShare>()
                .HasOne(ds => ds.ShareByUser)
                .WithMany(u => u.DocumentsSharedBy)
                .HasForeignKey(ds => ds.ShareByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DocumentShare>()
                .HasOne(ds => ds.ShareWithUser)
                .WithMany(u => u.DocumentsSharedWith)
                .HasForeignKey(ds => ds.ShareWithUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Owner)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(u => u.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }


    }
}
