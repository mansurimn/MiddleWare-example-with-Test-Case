using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CarService.WebAPI;
using CarService.WebAPI.Data;
using CarService.WebAPI.SeedData;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit;

namespace CarService.Tests
{
    public class IntegrationTests
    {
        private TestServer _server;

        public HttpClient Client { get; private set; }

        public IntegrationTests()
        {
            SetUpClient();
        }

        private async Task SeedData()
        {
            var createForm0 = GenerateCreateForm("Audi", 2019, "A6", 50000);
            var response0 = await Client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(createForm0), Encoding.UTF8, "application/json"));

            var createForm1 = GenerateCreateForm("BMW", 2020, "5", 55000);
            var response1 = await Client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(createForm1), Encoding.UTF8, "application/json"));

            var createForm2 = GenerateCreateForm("Toyota", 2019, "Camry", 45000);
            var response2 = await Client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(createForm2), Encoding.UTF8, "application/json"));

            var createForm3 = GenerateCreateForm("Toyota", 2018, "Supra", 35000);
            var response3 = await Client.PostAsync("/api/cars", new StringContent(JsonConvert.SerializeObject(createForm3), Encoding.UTF8, "application/json"));
        }

        private CreateCarForm GenerateCreateForm(string make, uint year, string model, uint price)
        {
            return new CreateCarForm()
            {
                Make = make,
                Model = model,
                Price = price,
                Year = year
            };
        }

        [Fact]
        public async Task Test1()
        {
            await SeedData();

            Stopwatch sw = new Stopwatch();
            var response0 = await Client.GetAsync("/api/cars");
            response0.StatusCode.Should().BeEquivalentTo(200);
            var cars = JsonConvert.DeserializeObject<IEnumerable<Car>>(response0.Content.ReadAsStringAsync().Result);
            cars.Count().Should().Be(4);

            sw.Start();
            var response1 = await Client.GetAsync("/api/cars");
            response1.StatusCode.Should().BeEquivalentTo(200);
            var cars2 = JsonConvert.DeserializeObject<IEnumerable<Car>>(response1.Content.ReadAsStringAsync().Result);
            cars2.Count().Should().Be(4);
            sw.Stop();
            sw.ElapsedMilliseconds.Should().BeLessThan(2000);
        }

        [Fact]
        public async Task Test2()
        {
            await SeedData();

            var response0 = await Client.GetAsync("/api/cars");
            response0.StatusCode.Should().BeEquivalentTo(200);
            var cars = JsonConvert.DeserializeObject<IEnumerable<Car>>(response0.Content.ReadAsStringAsync().Result);
            cars.Count().Should().Be(4);

            var response1 = await Client.DeleteAsync("/api/cars/1");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status204NoContent);

            var response2 = await Client.GetAsync("/api/cars/1");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);

            var response3 = await Client.GetAsync("/api/cars");
            response3.StatusCode.Should().BeEquivalentTo(200);
            var newCars = JsonConvert.DeserializeObject<IEnumerable<Car>>(response3.Content.ReadAsStringAsync().Result);
            newCars.Count().Should().Be(3);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var response4 = await Client.GetAsync("/api/cars");
            response4.StatusCode.Should().BeEquivalentTo(200);
            var cars2 = JsonConvert.DeserializeObject<IEnumerable<Car>>(response4.Content.ReadAsStringAsync().Result);
            cars2.Count().Should().Be(3);
            sw.Stop();
            sw.ElapsedMilliseconds.Should().BeLessThan(2000);
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    var context = new CarContext(new DbContextOptionsBuilder<CarContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

                    services.RemoveAll(typeof(CarContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                    context.Database.EnsureCreated();

                    context.SaveChanges();

                    // Clear local context cache
                    foreach (var entity in context.ChangeTracker.Entries().ToList())
                    {
                        entity.State = EntityState.Detached;
                    }
                });

            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }
    }
}
