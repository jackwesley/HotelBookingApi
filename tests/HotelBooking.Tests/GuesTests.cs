using HotelBooking.Domain.Models;
using Xunit;

namespace HotelBooking.Tests
{
    public class GuesTests
    {
        [Fact]
        public void Gues_WithCorrectData_ShouldBeValid()
        {
            //Arrange
            var reservation = new Guest("Jack", "123456", "jack@wesley.com.br", "phone");
            
            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.True(isValid);
        }
    }
}
