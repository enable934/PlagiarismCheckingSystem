using Microsoft.EntityFrameworkCore;
using PlagiarismCheckingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlagiarismCheckingSystem.Data
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options):base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<LaboratoryWork> LaboratoryWork { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<File> Files { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasOne(a => a.LaboratoryWork)
                .WithMany(b => b.Files)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LaboratoryWork>()
                .HasMany(a => a.Files)
                .WithOne(b => b.LaboratoryWork)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<File>()
                .HasMany(a => a.Similarities)
                .WithOne(b => b.File)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
