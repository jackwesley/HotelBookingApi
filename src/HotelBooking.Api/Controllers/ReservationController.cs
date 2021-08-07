using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.Controllers;
using HotelBooking.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
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
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseResult<ReservationDto>), description: "Data from reservation availability.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ResponseResult<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ResponseResult<string>), description: "Internal server error.")]
        public async Task<IActionResult> CheckAvailability(DateTime checkIn, DateTime checkOut)
        {
            return CustomResponse(await _reservationService.CheckAvailabilityAsync(checkIn, checkOut));
        }

        [HttpPost]
        [Route("reservation/place-reservation")]
        [SwaggerResponse(statusCode: 201, type: typeof(ResponseResult<ReservationDto>), description: "Placed Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ResponseResult<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult<string>), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ResponseResult<string>), description: "Internal server error.")]

        public async Task<IActionResult> PlaceReservationAsync([FromBody] ReservationDto reservationDto)
        {
            if (reservationDto == null)
                return CustomResponse(await _reservationService.PlaceReservationAsync(reservationDto));

            return CustomResponse(ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.NotFound, "Reservation must be provided", null));
        }

        [HttpDelete]
        [Route("reservation/cancel-reservation")]
        [SwaggerResponse(statusCode: 204, description: "Canceled Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ResponseResult<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult<string>), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ResponseResult<string>), description: "Internal server error.")]
        public async Task<IActionResult> CancelReservationAsync(Guid guestId, DateTime checkIn)
        {
            if (Guid.Empty != guestId)
                return CustomResponse(await _reservationService.CancelReservationAsync(guestId, checkIn));

            return CustomResponse(ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.NotFound, "Guest Id must be provided", null));
        }

        [HttpPut]
        [Route("reservation/modify-reservation")]
        [SwaggerResponse(statusCode: 200, type: typeof(ResponseResult<ReservationDto>), description: "Modified Reservation.")]
        [SwaggerResponse(statusCode: 400, type: typeof(ResponseResult<string>), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(ResponseResult<string>), description: "Not found.")]
        [SwaggerResponse(statusCode: 500, type: typeof(ResponseResult<string>), description: "Internal server error.")]
        public async Task<IActionResult> UpdateReservationAsync([FromBody] UpdateReservationDto updateReservationDto)
        {
            if (updateReservationDto != null && Guid.Empty != updateReservationDto.GuestId)
                return CustomResponse(await _reservationService.ModifyReservationAsync(updateReservationDto));

            return CustomResponse(ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.BadRequest, "Reservation must be provided.", null));
        }
    }
}
