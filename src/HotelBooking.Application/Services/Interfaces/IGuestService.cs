using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IGuestService
    {
        Task<ResponseResult> CreateGuestAsync(GuestDto guest);

        Task<ResponseResult> GetGuestByEmailAsync(string email);
    }
}
