using Microsoft.EntityFrameworkCore;

namespace LAN_File_Sharing_Application.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<UserAccount> Users { get; set; }
        public DbSet<userFolder> UserFolders { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<FileItem> FileItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bucket>(entity =>
            {
                entity.HasMany(f => f.Folders)
                    .WithOne(f => f.bucket)
                    .HasForeignKey(fo => fo.BucketID)
                    .IsRequired();
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasOne(b => b.bucket)
                    .WithOne(u => u.Account)
                    .HasForeignKey<Bucket>(fo => fo.AccountID)
                    .IsRequired();
            });

            modelBuilder.Entity<userFolder>(entity =>
            {
                entity.HasMany(i => i.Images)
                    .WithOne(f => f.Folder)
                    .HasForeignKey(fo => fo.FolderID)
                    .IsRequired();
            });
            modelBuilder.Entity<FileItem>(entity =>
            {
                entity.HasMany(i => i.Images)
                      .WithOne(f => f.FileItem)
                      .HasForeignKey(f => f.FileItemId)
                      .IsRequired(false);
            });
        }
    }
}
