using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IGuestService
    {
        Task<ResponseResult<GuestDto>> CreateGuestAsync(GuestDto guest);

        Task<ResponseResult<GuestDto>> GetGuestByEmailAsync(string email);
    }
}
