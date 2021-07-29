using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Repositories
{
    public interface IGuestRepository : IRepository<Guest>
    {
        Task<Guest> GetByEmail(string guestId);

        void AddGuest(Guest guest);
    }
}
