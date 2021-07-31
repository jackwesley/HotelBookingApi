using AutoFixture;
using FluentAssertions;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace HotelBooking.Tests.Application
{
    public class GuestServiceTests
    {
        private Mock<IGuestRepository> _guestRepository;
        private Mock<ILogger<GuestService>> _logger;
        private Fixture _fixture;
        private GuestService _service;
        public GuestServiceTests()
        {
            _fixture = new();
            _guestRepository = new();
            _logger = new();
            _service = new GuestService(_guestRepository.Object, _logger.Object);
        }

        [Fact]
        public async Task CreateGuest_WithInvalidData_ShouldReturnResponseResultWithBadRequest()
        {
            //Arrange
            var guestEntity = _fixture.Build<GuestDto>().Create();
            guestEntity.Name = string.Empty;

            //Act
            var response = await _service.CreateGuest(guestEntity);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateGuest_WithValidData_ShouldReturnResponseResultWithBadRequest()
        {
            //Arrange
            var guestEntity = _fixture.Build<GuestDto>().Create();
            
            _guestRepository.Setup(x => x.UnitOfWork.Commit())
                .ReturnsAsync(true);

            //Act
            var response = await _service.CreateGuest(guestEntity);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType(typeof(ResponseResult));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateGuest_WithException_ShouldLogAndReturInternalServerError()
        {
            //Arrange
            var guestEntity = _fixture.Build<GuestDto>().Create();

            _guestRepository.Setup(x => x.UnitOfWork.Commit())
                .Throws(new Exception("error saving guest"));

            //Act
            var response = await _service.CreateGuest(guestEntity);

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
    }
}
