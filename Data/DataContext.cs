using datingapp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>().HasKey(e => new { e.LikeeId, e.LikerId });
            modelBuilder.Entity<Like>().HasOne(e => e.Liker).WithMany(e => e.Likees)
            .HasForeignKey(e => e.LikerId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>().HasOne(e => e.Likee).WithMany(e => e.Likers)
            .HasForeignKey(e => e.LikeeId).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Message>().HasOne(e => e.Sender).WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>().HasOne(e => e.Reciver).WithMany(e => e.MessagesRecived)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}