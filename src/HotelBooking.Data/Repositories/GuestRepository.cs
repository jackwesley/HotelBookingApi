using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelBooking.Data.Repositories
{
    public class GuestRepository : IGuestRepository
    {
        private readonly ReservationContext _context;

        public GuestRepository(ReservationContext reservationContext)
        {
            _context = reservationContext;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Guest> GetByEmail(string email)
        {
            return await _context.Guests.FirstOrDefaultAsync(x => x.Email.Equals(email));
        }

        public async Task AddGuest(Guest guest)
        {
            await _context.Guests.AddAsync(guest);
        }

        
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
