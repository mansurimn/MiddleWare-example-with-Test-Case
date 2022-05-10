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
          //var cars=new List<cars>;
          List<car> cars;
          //bool isExist = await _cache.TryGetValue("cars", out cars); 
          var cars =  _cache.GetFromCache<IEnumerable<car>>(cacheKey);

          if (cars==null) 
             {
             Task.Delay(2000);             
            var cacheEntryOptions = new MemoryCacheEntryOptions()  
            .SetSlidingExpiration(TimeSpan.FromDay(1));  
            cars = await _carsService.Get(null, filters);
           _cache.Set("cars", currentTime, cacheEntryOptions);  
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
            var user = (await _carsService.Get(new[] { id }, null)).FirstOrDefault();
            if (user == null)
                return NotFound();
            var cars =  _cache.GetFromCache<IEnumerable<car>>(cacheKey);
            if(cars!=null)
            {
                //cars.Remove();
            }
            await _carsService.Delete(user);
            return NoContent();
        }
    }
}
