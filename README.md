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
    {
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HotelBookingDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
}
```
After the confiration mentioned above, open the console, choose the project HotelBooking.Data and apply the changes for the database running the follow command:

![image](https://user-images.githubusercontent.com/16781964/127750238-402eaf5b-ecaa-485f-91ae-d22a3c52cd63.png)

##Tools utilized
1. Swagger for the API documentation
2. Entity Framework Core to connect with DataBase
3. Fluent Validation for creation of validation for Domain Classes
4. Auto Fixture to create class objects in tests
5. Fluent Assertions to assert response from tests
6. Moq to moq interfaces in tests

## Links of the Application
1. Swagger -> https://localhost:44331/swagger/index.html

## How to use the API.
This API can be used directly via Swagger, in link mentioned before.
It is possible to use programs like postman to call the API as well.
When calling the endpoints, be sure to create an Guest and keep it Id, the others endpoints will need it.

The project folter contains a 
