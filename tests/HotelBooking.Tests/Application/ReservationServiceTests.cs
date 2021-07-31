using AutoFixture;
using FluentAssertions;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace HotelBooking.Tests.Application
{
    public class ReservationServiceTests
    {
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<ILogger<ReservationService>> _logger;
        private Fixture _fixture;
        private ReservationService _service;
        public ReservationServiceTests()
        {
            _fixture = new();
            _reservationRepository = new();
            _logger = new();
            _service = new ReservationService(_reservationRepository.Object, _logger.Object);
        }

        #region CheckAvailabilityTests
        [Fact]
        public void CheckAvailability_WithCheckinGreaterThanCheckOut_ShouldReturn_BadRequest()
        {
            //Arrange
            DateTime checkIn = DateTime.Now.AddDays(2);
            DateTime checkOut = DateTime.Now;

            //Act
            var response = _service.CheckAvailability(checkIn, checkOut);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CheckAvailability_WithDiffBetweenCheckinAndCheckOutGreaterThan3Days_ShouldReturn_BadRequest()
        {
            //Arrange
            DateTime checkIn = DateTime.Now;
            DateTime checkOut = DateTime.Now.AddDays(4);

            //Act
            var response = _service.CheckAvailability(checkIn, checkOut);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void CheckAvailability_WithCheckingAndCheckoutOkWithAvailableRoom_ShouldReturn_RoomsAvailable()
        {
            //Arrange
            DateTime checkIn = DateTime.Now;
            DateTime checkOut = DateTime.Now.AddDays(2);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>())).Returns(true);

            //Act
            var response = _service.CheckAvailability(checkIn, checkOut);
            var responseMessage = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            responseMessage.Should().Contain("Room is available for CheckIn");
        }

        [Fact]
        public void CheckAvailability_WithCheckingAndCheckoutOkWithUnavailableRoom_ShouldReturn_RoomsNotAvailable()
        {
            //Arrange
            DateTime checkIn = DateTime.Now;
            DateTime checkOut = DateTime.Now.AddDays(2);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>())).Returns(false);

            //Act
            var response = _service.CheckAvailability(checkIn, checkOut);
            var responseMessage = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            responseMessage.Should().Contain("Room is NOT available for CheckIn");
        }
        #endregion

        #region PlaceReservationTests
        [Fact]
        public async Task PlaceReservation_WithAvailableDates_ShouldReturn_ReservationDone()
        {
            //Arrange
            var reservationDto = _fixture.Build<ReservationDto>().Create();
            reservationDto.CheckIn = DateTime.Now.AddDays(1);
            reservationDto.CheckOut = DateTime.Now.AddDays(2);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(true);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            //Act
            var response = await _service.PlaceReservationAsync(reservationDto);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PlaceReservation_WithUnAvailableDates_ShouldReturn_CheckinNotAvailable()
        {
            //Arrange
            var reservationDto = _fixture.Build<ReservationDto>().Create();
            reservationDto.CheckIn = DateTime.Now.AddDays(1);
            reservationDto.CheckOut = DateTime.Now.AddDays(2);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(false);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            //Act
            var response = await _service.PlaceReservationAsync(reservationDto);
            var responseMessage = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseMessage.Should().Contain("Room is NOT available for CheckIn");
        }

        [Fact]
        public async Task PlaceReservation_WithAvailableDatesThrowingException_ShouldLogAndReturn_InternalServerError()
        {
            //Arrange
            var reservationDto = _fixture.Build<ReservationDto>().Create();
            reservationDto.CheckIn = DateTime.Now.AddDays(1);
            reservationDto.CheckOut = DateTime.Now.AddDays(2);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(true);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .Throws(new Exception());

            //Act
            var response = await _service.PlaceReservationAsync(reservationDto);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            _logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Exactly(1));
        }
        #endregion

        #region ModifyReservationTests
        [Fact]
        public async Task ModifyReservation_WithAvailableDates_ShouldReturn_ModfyDoneWithStatusOk()
        {
            //Arrange
            var updateReservationDto = _fixture.Build<UpdateReservationDto>().Create();
            updateReservationDto.CurrentCheckIn = DateTime.Now.AddDays(1);
            updateReservationDto.CurrentCheckOut = DateTime.Now.AddDays(2);
            updateReservationDto.NewCheckIn = DateTime.Now.AddDays(3);
            updateReservationDto.NewCheckOut = DateTime.Now.AddDays(4);

            var reservation = new Reservation(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn, updateReservationDto.CurrentCheckOut);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(true);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(true);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.ModifyReservation(updateReservationDto);
            var reservationResponse = response.Response as ReservationDto;
            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            reservationResponse.CheckIn.Should().Equals(updateReservationDto.NewCheckIn);
            reservationResponse.CheckOut.Should().Equals(updateReservationDto.NewCheckOut);
        }

        [Fact]
        public async Task ModifyReservation_WithAvailableDatesNotCommitedByUnitOfWork_ShouldReturn_InternalServerError()
        {
            //Arrange
            var updateReservationDto = _fixture.Build<UpdateReservationDto>().Create();
            updateReservationDto.CurrentCheckIn = DateTime.Now.AddDays(1);
            updateReservationDto.CurrentCheckOut = DateTime.Now.AddDays(2);
            updateReservationDto.NewCheckIn = DateTime.Now.AddDays(3);
            updateReservationDto.NewCheckOut = DateTime.Now.AddDays(4);

            var reservation = new Reservation(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn, updateReservationDto.CurrentCheckOut);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(true);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .ReturnsAsync(false);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.ModifyReservation(updateReservationDto);
            
            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task ModifyReservation_WithAvailableDatesThrowingException_ShouldCallLogReturn_InternalServerError()
        {
            //Arrange
            var updateReservationDto = _fixture.Build<UpdateReservationDto>().Create();
            updateReservationDto.CurrentCheckIn = DateTime.Now.AddDays(1);
            updateReservationDto.CurrentCheckOut = DateTime.Now.AddDays(2);
            updateReservationDto.NewCheckIn = DateTime.Now.AddDays(3);
            updateReservationDto.NewCheckOut = DateTime.Now.AddDays(4);

            var reservation = new Reservation(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn, updateReservationDto.CurrentCheckOut);

            _reservationRepository.Setup(x => x.CheckAvailabilyForDates(It.IsAny<List<DateTime>>()))
                .Returns(true);
            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
               .Throws(new Exception());

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.ModifyReservation(updateReservationDto);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            _logger.Verify(l => l.Log(
               LogLevel.Error,
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
               Times.Exactly(1));
        }
        #endregion

        #region CancelReservationTests
        [Fact]
        public async Task CancelReservation_WithReservationExisting_ShouldReturn_ModfyDoneWithStatusOk()
        {
            //Arrange
            Guid guestId = Guid.NewGuid();
            var checkin = DateTime.Now;
            var reservation = new Reservation(guestId, DateTime.Now, DateTime.Now.AddDays(2));

            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
              .ReturnsAsync(true);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(guestId, checkin))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.CancelReservation(guestId, checkin);
            var reservationResponse = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            reservationResponse.Should().Contain("Reservation canceled successfully");
        }

        [Fact]
        public async Task CancelReservation_WithReservationExisting_ShouldReturn_ModfyDoneWithStatusNoContent()
        {
            //Arrange
            Guid guestId = Guid.NewGuid();
            var checkin = DateTime.Now;
            var reservation = new Reservation(guestId, DateTime.Now, DateTime.Now.AddDays(2));

            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
              .ReturnsAsync(true);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(guestId, checkin))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.CancelReservation(guestId, checkin);
            var reservationResponse = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            reservationResponse.Should().Contain("Reservation canceled successfully");

        }

        [Fact]
        public async Task CancelReservation_WithReservationNotExisting_ShouldReturn_NotFound()
        {
            //Arrange
            Guid guestId = Guid.NewGuid();
            var checkin = DateTime.Now;

            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
              .ReturnsAsync(true);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(guestId, checkin))
                .ReturnsAsync(null as Reservation);

            //Act
            var response = await _service.CancelReservation(guestId, checkin);
            var reservationResponse = response.Response as string;

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            reservationResponse.Should().Contain("Reservation not found.");
        }

        [Fact]
        public async Task CancelReservation_WithReservationExistingAndUOWReturningFalse_ShouldReturn_InternalServerError()
        {
            //Arrange
            Guid guestId = Guid.NewGuid();
            var checkin = DateTime.Now;
            var reservation = new Reservation(guestId, DateTime.Now, DateTime.Now.AddDays(2));

            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
              .ReturnsAsync(false);

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(guestId, checkin))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.CancelReservation(guestId, checkin);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task CancelReservation_WithReservationExistingAndUOWThrowingException_ShouldCallLogAndReturn_InternalServerError()
        {
            //Arrange
            Guid guestId = Guid.NewGuid();
            var checkin = DateTime.Now;
            var reservation = new Reservation(guestId, DateTime.Now, DateTime.Now.AddDays(2));

            _reservationRepository.Setup(x => x.UnitOfWork.Commit())
              .Throws(new Exception());

            _reservationRepository.Setup(x => x.GetByGuestIdAndCheckinAsync(guestId, checkin))
                .ReturnsAsync(reservation);

            //Act
            var response = await _service.CancelReservation(guestId, checkin);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            _logger.Verify(l => l.Log(
              LogLevel.Error,
              It.IsAny<EventId>(),
              It.IsAny<It.IsAnyType>(),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
              Times.Exactly(1));
        }
        #endregion
    }
}
