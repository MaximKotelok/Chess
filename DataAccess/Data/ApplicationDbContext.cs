using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        public DbSet<User>? Users { get; set; }
        public DbSet<UserFriend>? UserFriends { get; set; }
        public DbSet<Session>? Sessions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Session>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<UserFriend>()
                .HasKey(uf => new { uf.FirstUserId, uf.SecondSecondId });

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.FirstUser)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.FirstUserId);

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.SecondUser)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.SecondSecondId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.White)
                .WithMany(u => u.Sessions)
                .HasForeignKey(uf => uf.WhiteId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Black)
                .WithMany(u => u.Sessions)
                .HasForeignKey(uf => uf.BlackId);


        }

    }

}
