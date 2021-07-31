using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IActionResult AvailabilityForCurrentMonth(DateTime checkIn, DateTime checkOut)
        {
            return CustomResponse(_reservationService.CheckAvailability(checkIn, checkOut));
        }

        [HttpPost]
        [Route("reservation/place-reservation")]
        public async Task<IActionResult> PlaceReservation([FromBody] ReservationDto reservationDto)
        {
            return CustomResponse(await _reservationService.PlaceReservationAsync(reservationDto));
        }

        [HttpDelete]
        [Route("reservation/cancel-reservation")]
        public async Task<IActionResult> CancelReservation(Guid userId, DateTime checkIn)
        {
            return CustomResponse(await _reservationService.CancelReservation(userId, checkIn));
        }

        [HttpPut]
        [Route("reservation/modify-reservation")]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDto updateReservationDto)
        {
            return CustomResponse(await _reservationService.ModifyReservation(updateReservationDto));
        }
    }
}
