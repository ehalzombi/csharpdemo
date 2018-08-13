using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;

namespace HHParser.Models
{
    public class HHDatabaseContext : DbContext
    {
        public HHDatabaseContext(DbContextOptions<HHDatabaseContext> opts)
            : base(opts) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var uriConverter = new ValueConverter<Uri, string>(
                u => u.ToString(),
                u => new Uri(u)
                );

            modelBuilder
                .Entity<Category>()
                .Property(c => c.Url)
                .HasConversion(uriConverter);

            modelBuilder
                .Entity<Subcategory>()
                .Property(c => c.Url)
                .HasConversion(uriConverter);

            modelBuilder
                .Entity<Vacancy>()
                .Property(c => c.Url)
                .HasConversion(uriConverter);

        }
    }

}
