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

        [Fact]
        public void Gues_WithNameNullOrEmpty_ShouldBeInValid()
        {
            //Arrange
            var reservation = new Guest(string.Empty, "123456", "jack@wesley.com.br", "phone");

            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Gues_WithDocumentNullOrEmpty_ShouldBeInValid()
        {
            //Arrange
            var reservation = new Guest("Jack", string.Empty, "jack@wesley.com.br", "phone");

            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Gues_WithEmailNullOrEmpty_ShouldBeInValid()
        {
            //Arrange
            var reservation = new Guest("Jack", "13456", string.Empty, "phone");

            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Gues_WithPhoneNullOrEmpty_ShouldBeInValid()
        {
            //Arrange
            var reservation = new Guest("Jack", "13456", "jack@mail.com", string.Empty);

            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }
    }
}
