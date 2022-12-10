using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Pomelo.EntityFrameworkCore.MySql;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace Weather
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Models.City> Cities { get; set; } = null!;
        public DbSet<Models.CityWeather> CityWeathers { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.City>().HasData(
                new Models.City { Id = 1, Name = "bryansk" }
            );

            modelBuilder.Entity<Models.CityWeather>().Property(p => p.Temp).HasPrecision(4, 2);

        }
    }
}
