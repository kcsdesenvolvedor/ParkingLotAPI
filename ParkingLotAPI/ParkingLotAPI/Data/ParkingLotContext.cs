using Microsoft.EntityFrameworkCore;
using ParkingLotAPI.Entities;

namespace ParkingLotAPI.Data
{
    public class ParkingLotContext : DbContext
    {
        public DbSet<Car> Cars {  get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Parking> Parkings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "parking-lot.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
