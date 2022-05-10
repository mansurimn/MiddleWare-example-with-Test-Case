using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CarService.WebAPI.Data
{
    public class CarContext : DbContext
    {
        public CarContext(DbContextOptions<CarContext> options)
            : base(options)
        { }

        public DbSet<Car> Cars { get; set; }
    }

    public class Car
    {
        [Key]
        public int Id { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public uint Price { get; set; }

        public uint Year { get; set; }
    }
}
