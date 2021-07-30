using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IGuestService
    {
        Task<ResponseResult> CreateGuest(GuestDto guest);

        Task<ResponseResult> GetGuestByEmail(string email);
    }
}
