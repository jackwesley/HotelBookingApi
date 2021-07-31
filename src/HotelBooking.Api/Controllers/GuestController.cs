using FluentValidation.Results;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.Controllers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Api.Controllers
{
    public class GuestController : MainController
    {

        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        [Route("guest/{email}")]
        [SwaggerResponse(statusCode: 200, type: typeof(GuestDto), description: "Data from Guest.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        public async Task<IActionResult> GetGuestAsync(string email)
        {

            return CustomResponse(await _guestService.GetGuestByEmailAsync(email));
        }

        [HttpPost]
        [Route("guest")]
        [SwaggerResponse(statusCode: 200, type: typeof(GuestDto), description: "Data from Guest.")]
        [SwaggerResponse(statusCode: 400, type: typeof(IEnumerable<string>), description: "Bad request.")]
        public async Task<IActionResult> CreateGuestAsync([FromBody] GuestDto guestDto)
        {
            return CustomResponse(await _guestService.CreateGuestAsync(guestDto));
        }
    }
}
