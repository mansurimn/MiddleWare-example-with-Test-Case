using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarService.WebAPI.Data;
using CarService.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace CarService.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
       private string cacheKey="cars";
        private readonly ICarsService _carsService;
        private readonly IMemoryCache _cache;
        public CarsController(ICarsService carsService,IMemoryCache memoryCache)
        {
            _carsService = carsService;
            _cache=memoryCache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = (await _carsService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] Filters filters)
        {
            var cars =  _cache.Get<IEnumerable<Car>>(cacheKey);
            if (cars==null)
            {
                await Task.Delay(2000);
                cars = await _carsService.Get(null, filters);
                _cache.Set(cacheKey, cars, DateTimeOffset.Now.AddDays(1));
            }    
            return Ok(cars);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Car car)
        {
            await _carsService.Add(car);
            return Ok(car);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var car = (await _carsService.Get(new[] { id }, null)).FirstOrDefault();
            if (car == null)
                return NotFound();
            var cars =  _cache.Get(cacheKey) as List<Car>;
            if(cars!=null)
            {
                cars.Remove(car);
                _cache.Remove(cacheKey);
                _cache.Set(cacheKey, cars, DateTimeOffset.Now.AddDays(1));
            }
            await _carsService.Delete(car);
            return NoContent();
        }
    }
}
