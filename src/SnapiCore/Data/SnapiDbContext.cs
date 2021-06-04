using System;
using Microsoft.EntityFrameworkCore;
using SnapiCore.Data.Models;

namespace SnapiCore.Data
{
    /// <summary>
    /// Контекст справочников.
    /// </summary>
    public class SnapiDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SubscriberLink> Subscribers { get; set; }

        public static SnapiDbContext Build()
        {
            var builder = new DbContextOptionsBuilder<SnapiDbContext>();
            ConfigureBuilder(builder, @"Data Source=test.db");
            return new SnapiDbContext(builder.Options);
        }
        
        public static DbContextOptionsBuilder ConfigureBuilder(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlite(connectionString);
            builder.EnableSensitiveDataLogging();
            builder.LogTo(Console.WriteLine);
            return builder;
        }

        public SnapiDbContext(DbContextOptions<SnapiDbContext> context) : base(context)
        {
        }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubscriberLink>()
                .HasOne(p => p.From)
                .WithMany(b => b.Subscriptions);
            modelBuilder.Entity<SubscriberLink>()
                .HasOne(p => p.To)
                .WithMany(b => b.Subscribers);
            modelBuilder.Entity<User>()
                .HasIndex(x => x.IndexName)
                .IsUnique();
            modelBuilder.Entity<SubscriberLink>()
                .HasIndex(x =>
                new {
                    x.FromId,
                    x.ToId
                });
        }
    }
}