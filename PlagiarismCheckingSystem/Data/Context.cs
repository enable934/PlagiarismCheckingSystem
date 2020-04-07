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
    }
}
