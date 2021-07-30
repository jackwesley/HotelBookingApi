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
        [Route("reservation/check-availability-for-next-30-days")]
        public async Task<IActionResult> Availability()
        {
            return CustomResponse(await _reservationService.CheckAvailabilityForTheMonth());
        }

        [HttpPost]
        [Route("reservation")]
        public async Task<IActionResult> PlaseReservation([FromBody] ReservationDto reservationDto)
        {
            return CustomResponse(await _reservationService.PlaceReservation(reservationDto));
        }

        [HttpDelete]
        [Route("reservation")]
        public async Task<IActionResult> CancelReservation(Guid userId, DateTime checkInDate)
        {
            return CustomResponse(await _reservationService.CancelReservation(userId, checkInDate));
        }

        [HttpPut]
        [Route("reservation")]
        public async Task<IActionResult> UpdateReservation(Guid userId, DateTime oldCheckInDate, DateTime newCheckinDate, DateTime oldCheckOutDate, DateTime newCheckoutDate)
        {
            return CustomResponse(await _reservationService.ModifyReservation(userId, oldCheckInDate, newCheckinDate, oldCheckOutDate, newCheckoutDate));
        }
    }
}
