using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotelBooking.Api.Controllers
{
    public class GuestController : Controller
    {

        private readonly IGuestService _guestService;

        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet]
        [Route("guest/{email}")]
        public async Task<IActionResult> GetGuest(string email)
        {
            var result = await _guestService.GetGuestByEmail(email);

            if (result.Response == null)
                return NotFound(result.ValidationResult.Errors);

            return Ok(result.Response);
        }

        [HttpPost]
        [Route("guest")]
        public async Task<IActionResult> CreateGuest([FromBody] GuestDto guestDto)
        {
            var result = await _guestService.CreateGuest(guestDto);

            if (result.ValidationResult == null && result.ValidationResult.Errors == null)
                return Ok(result.Response);
            else
                return NotFound(result.ValidationResult.Errors);
        }
    }
}
