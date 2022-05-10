## Environment:  
- .NET version: 3.0

## Read-Only Files:   
- CarsService.Tests/IntegrationTests.cs
- CarsService.WebAPI/Services/CarsService.cs

## Data:  
Example of a car data JSON object:
```
{
    id: 5,
    make: "Audi",
    model: "A6",
    price: "76000",
    year: 2018
} 
```

## Requirements:

A company is launching a service that can manage cars. The service should be a web API layer using .NET Core 3.0. You already have a prepared infrastructure and need to implement a Web API Controller CarsController. Also, you need to think about performance. The service endpoint for getting all available cars potentially can take a long time in case of large data. Therefore, you need to implement caching for this endpoint using the in-memory caching mechanism from .NET Core.



The following API calls are already implemented:
1. Creating cars: a POST request to the endpoint api/cars adds a car to the database. The HTTP response code is 200.
2. Getting all cars: a GET request to the endpoint api/cars returns the entire list of cars. The HTTP response code is 200.
3. Getting cars item by id: a GET request to the endpoint api/cars/{id} should return the details of the car for the {id}. If there is no car for the {id}, status code 404 is returned. On success, status code 200 is returned.
4. Getting all cars filtered by the years property: GET request to the endpoint api/cars?years={year1}&years={year2} should return the entire list of cars for year1 and year2. The HTTP response code is 200.
5. Deleting a car by id: a DELETE request to the endpoint api/cars/{id} should delete the corresponding car. If there is no car for {id}, status code 404 is returned. On success, status code 200 is returned.


Change the API endpoints of the project in the following way:
- For the "Getting all cars" endpoint, you need to think about performance. The first query to GET hits the database. For the second query to GET all cars, you need to get the response faster using the .NET Core in-memory cache mechanism. To perceive the difference between the first and second requests, the service takes care of adding a delay of 2 seconds to imitate a long query to the database.
- Important: Any operation that changes the cars list should delete the in-memory cache. Tests take care of testing this.


Definition of News model:
+ id - The ID of the car. [INTEGER]
+ make - The make of the car. [STRING]
+ model - The model of the car. [STRING]
+ price - The price of the car. [INTEGER]
+ year - The car's production year. [INTEGER]


## Example requests and responses with headers


**Request 1:**

GET request to api/cars

The response code will be 200 with a list of the car's details with a time delay of 2 seconds:
```
[
{
    id: 5,
    make: "Audi",
    model: "A6",
    price: "76000",
    year: 2018
} 
]
```


**Request 2:**

GET request to api/cars

The response code will be 200 with a list of the car's details without a time delay of 2 seconds because the memory cache mechanism was used:
```
[
{
    id: 5,
    make: "Audi",
    model: "A6",
    price: "76000",
    year: 2018
} 
]
```

**Request 3:**

DELETE request to api/cars/1

The response code will be 204, and this should clean the memory cache of the news feed.
