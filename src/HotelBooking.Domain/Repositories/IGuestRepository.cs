using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Repositories
{
    public interface IGuestRepository : IRepository<Guest>
    {
        Task<Guest> GetByEmailAsync(string guestId);

        Task AddGuestAsync(Guest guest);
    }
}
