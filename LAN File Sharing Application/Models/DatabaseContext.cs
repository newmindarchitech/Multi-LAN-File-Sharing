using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace LAN_File_Sharing_Application.Models
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> context) : base(context)
        {

        }
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<userFolder> UserFolders { get; set; }

        public DbSet<Bucket> Buckets { get; set; }

        public DbSet<Images> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);



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
                entity.HasMany(i=>i.Images)
                .WithOne(f=>f.Folder)
                .HasForeignKey(fo=>fo.FolderID)
                .IsRequired();
            });

        }
    }
}
