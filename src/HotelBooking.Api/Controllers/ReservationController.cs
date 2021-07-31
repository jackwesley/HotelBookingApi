using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.Controllers;
using HotelBooking.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Api.Controllers
{
    public class ReservationController : MainController
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Route("reservation/check-availability")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseResult), description: "Data from reservation availability.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 500, type: typeof(IEnumerable<string>), description: "Internal server error.")]
        public IActionResult AvailabilityForCurrentMonth(DateTime checkIn, DateTime checkOut)
        {
            return CustomResponse(_reservationService.CheckAvailability(checkIn, checkOut));
        }

        [HttpPost]
        [Route("reservation/place-reservation")]
        [SwaggerResponse(statusCode: 201, type: typeof(ResponseResult), description: "Placed Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(IEnumerable<string>), description: "Internal server error.")]

        public async Task<IActionResult> PlaceReservation([FromBody] ReservationDto reservationDto)
        {
            return CustomResponse(await _reservationService.PlaceReservationAsync(reservationDto));
        }

        [HttpDelete]
        [Route("reservation/cancel-reservation")]
        [SwaggerResponse(statusCode: 204, type: typeof(ResponseResult), description: "Canceled Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(IEnumerable<string>), description: "Internal server error.")]

        public async Task<IActionResult> CancelReservation(Guid userId, DateTime checkIn)
        {
            return CustomResponse(await _reservationService.CancelReservation(userId, checkIn));
        }

        [HttpPut]
        [Route("reservation/modify-reservation")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseResult), description: "Modified Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(IEnumerable<string>), description: "Internal server error.")]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDto updateReservationDto)
        {
            return CustomResponse(await _reservationService.ModifyReservation(updateReservationDto));
        }
    }
}
