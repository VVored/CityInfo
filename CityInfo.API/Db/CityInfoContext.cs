using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Db
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; }
        public CityInfoContext() { }
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one big with that big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with cathedral that was never really finished."
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with that big tower."
                });
            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1
                },
                new PointOfInterest("2")
                {
                    Id = 2,
                    CityId = 1
                },
                new PointOfInterest("3")
                {
                    Id = 3,
                    CityId = 2
                },
                new PointOfInterest("4")
                {
                    Id = 4,
                    CityId = 2
                },
                new PointOfInterest("5")
                {
                    Id = 5,
                    CityId = 3
                },
                new PointOfInterest("6")
                {
                    Id = 6,
                    CityId = 3
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}
