using HotelBooking.Domain.Models;
using System;
using Xunit;

namespace HotelBooking.Tests.Domain
{
    public class ReservationTests
    {
        [Fact]
        public void Reservation_WithCorrectInput_ShouldBeValid()
        {
            //Arrange
            var guestId = Guid.NewGuid();
            var checkin = DateTime.Now.AddDays(1);
            var checkout = DateTime.Now.AddDays(2);

            var reservation = new Reservation(guestId, checkin, checkout);
            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Reservation_WithMoreThan3Days_ShouldBeInValid()
        {
            //Arrange
            var guestId = Guid.NewGuid();
            var checkin = DateTime.Now.AddDays(1);
            var checkout = DateTime.Now.AddDays(5);

            var diffBetweenCheckinAndCheckoutInDays = checkout.Subtract(checkin).Days;

            var reservation = new Reservation(guestId, checkin, checkout);
            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Reservation_WithChecking30DaysAdvanced_ShouldBeInValid()
        {
            //Arrange
            var guestId = Guid.NewGuid();
            var checkin = DateTime.Now.AddDays(31);
            var checkout = DateTime.Now.AddDays(34);
            var diffBetweenCheckinAndCheckoutInDays = checkout.Subtract(checkin).Days;

            var reservation = new Reservation(guestId, checkin, checkout);
            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Reservation_WithCheckingInTheSameDay_ShouldBeInValid()
        {
            //Arrange
            var guestId = Guid.NewGuid();
            var checkin = DateTime.Now;
            var checkout = DateTime.Now.AddDays(3);
            var diffBetweenCheckinAndCheckoutInDays = checkout.Subtract(checkin).Days;

            var reservation = new Reservation(guestId, checkin, checkout);
            //Act
            var isValid = reservation.IsValid();

            //Assert
            Assert.False(isValid);
        }
    }
}
