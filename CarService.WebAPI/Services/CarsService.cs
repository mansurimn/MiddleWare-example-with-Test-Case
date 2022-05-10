using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarService.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CarService.WebAPI.Services
{
    public class CarsService : ICarsService
    {
        private readonly CarContext _carContext;

        public CarsService(CarContext carContext)
        {
            _carContext = carContext;
        }

        public async Task<IEnumerable<Car>> Get(int[] ids, Filters filters)
        {
            var cars = _carContext.Cars.AsQueryable();

            if (filters == null)
                filters = new Filters();

            if (filters.Years != null && filters.Years.Any())
                cars = cars.Where(x => filters.Years.Contains(x.Year));

            if (filters.Makes != null && filters.Makes.Any())
                cars = cars.Where(x => filters.Makes.Contains(x.Make));

            if (filters.Models != null && filters.Models.Any())
                cars = cars.Where(x => filters.Models.Contains(x.Model));

            if (ids != null && ids.Any())
                cars = cars.Where(x => ids.Contains(x.Id));

            await Task.Delay(2000);

            return await cars.ToListAsync();
        }

        public async Task<Car> Add(Car car)
        {
            await _carContext.Cars.AddAsync(car);
            await _carContext.SaveChangesAsync();
            return car;
        }

        public async Task<IEnumerable<Car>> AddRange(IEnumerable<Car> cars)
        {
            await _carContext.Cars.AddRangeAsync(cars);
            await _carContext.SaveChangesAsync();
            return cars;
        }

        public async Task<Car> Update(Car car)
        {
            var userForChanges = await _carContext.Cars.SingleAsync(x => x.Id == car.Id);
            userForChanges.Price = car.Price;
            userForChanges.Year = car.Year;
            userForChanges.Make = car.Make;
            userForChanges.Model = car.Model;

            _carContext.Cars.Update(userForChanges);
            await _carContext.SaveChangesAsync();
            return car;
        }

        public async Task<bool> Delete(Car car)
        {
            _carContext.Cars.Remove(car);
            await _carContext.SaveChangesAsync();

            return true;
        }
    }

    public interface ICarsService
    {
        Task<IEnumerable<Car>> Get(int[] ids, Filters filters);

        Task<Car> Add(Car car);

        Task<IEnumerable<Car>> AddRange(IEnumerable<Car> cars);

        Task<Car> Update(Car car);

        Task<bool> Delete(Car car);
    }

    public class Filters
    {
        public uint[] Years { get; set; }
        public string[] Makes { get; set; }
        public string[] Models { get; set; }
    }
}
