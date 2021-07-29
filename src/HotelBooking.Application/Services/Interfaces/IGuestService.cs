using HotelBooking.Application.DTOs;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IGuestService
    {
        Task<ResponseResult> CreateGuest(GuestDto guest);

        Task<ResponseResult> GetGuestByEmail(string email);
    }
}
