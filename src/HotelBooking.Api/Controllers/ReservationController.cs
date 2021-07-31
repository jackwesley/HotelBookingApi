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
        [Route("reservation/check-availability-for-current-month")]
        public async Task<IActionResult> AvailabilityForCurrentMonth()
        {
            return CustomResponse(await _reservationService.CheckAvailabilityForTheMonth());
        }

        [HttpPost]
        [Route("reservation/place-reservation")]
        public async Task<IActionResult> PlaceReservation([FromBody] ReservationDto reservationDto)
        {
            return CustomResponse(await _reservationService.PlaceReservation(reservationDto));
        }

        [HttpDelete]
        [Route("reservation/cancel-reservation")]
        public async Task<IActionResult> CancelReservation(Guid userId, DateTime checkInDate)
        {
            return CustomResponse(await _reservationService.CancelReservation(userId, checkInDate));
        }

        [HttpPut]
        [Route("reservation/modify-reservation")]
        public async Task<IActionResult> UpdateReservation([FromBody] UpdateReservationDto updateReservationDto)
        {
            return CustomResponse(await _reservationService.ModifyReservation(updateReservationDto));
        }
    }
}
