# HotelBookingApi
This is a API responsible for manage reservations in a hotel.

This API contains a SWAGGER documentation to help consumers understand how to use it.

Follow some important informations to build the application.

First is needed to have intalled in your machine:
1. Dot Net SDK for .Net(core) 5
2. Sql Server 

For Data Base Settings, make sure having the configuration below in the appsettings.json in the HotelBooking.Api project.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
}
```
After the confiration mentioned above, open the console, choose the project HotelBooking.Data and apply the changes for the database running the follow command:

![image](https://user-images.githubusercontent.com/16781964/127750238-402eaf5b-ecaa-485f-91ae-d22a3c52cd63.png)

## Tools utilized
1. Swagger for the API documentation
2. Entity Framework Core to connect with DataBase
3. Fluent Validation for creation of validation for Domain Classes
4. Auto Fixture to create class objects in tests
5. Fluent Assertions to assert response from tests
6. Moq to moq interfaces in tests

## Links of the Application
1. Swagger -> https://localhost:44331/swagger/index.html
2. Postman Collection -> https://www.getpostman.com/collections/589218b4bae11604d3f7

## How to use the API.
This API can be used directly via Swagger, in link mentioned before.
It is possible to use programs like postman to call the API as well.
When calling the endpoints, be sure to create an Guest and keep its Id, the others endpoints will need it.
The link of the postman collection is above can be used. It contais an example for each endpoint.

* Observation: All Data fields must be provided in format YYYY/MM/DD

## Architecture proposal
![image](https://user-images.githubusercontent.com/16781964/127753971-801e54fe-696d-43e7-9471-bb0e6990f07e.png)

It consists of publishing the Rest API and the database SQL Server in containers utilizing Kubernetes with HPA(Horizontal Pod Autoscaler).
