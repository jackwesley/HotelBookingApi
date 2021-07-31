using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Data.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationContext _context;
        public ReservationRepository(ReservationContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public void CancelReservation(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
        }

        public async Task<IEnumerable<Reservation>> GetAllCheckingForTheMonthAsync()
        {
            return await _context.Reservations
                .AsNoTracking().Where(x => x.CheckIn > DateTime.Now.AddDays(1) && x.CheckOut <= DateTime.Now.AddDays(30))
                .ToListAsync();
        }

        public async Task<Reservation> GetByGuestIdAsync(Guid guestId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.GuestId.Equals(guestId));
        }

        public async Task<Reservation> GetByGuestIdAndCheckinAsync(Guid guestId, DateTime checkin)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.GuestId.Equals(guestId) && x.CheckIn == checkin);
        }

        public async Task<Reservation> GetByCheckinAsync(DateTime checkin)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.CheckIn == checkin);
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsByGuestIdAsync(Guid guestId)
        {
            return await _context.Reservations
              .AsNoTracking().Where(x => x.GuestId == guestId)
              .ToListAsync(); ;
        }

        public bool CheckAvailabilyForDates(List<DateTime> datesToCheck)
        {
            var checkins = _context.Reservations.Where(x => datesToCheck.Contains(x.CheckIn.Date) || datesToCheck.Contains(x.CheckOut.Date));

            return checkins.ToList().Count == 0;
        }

        public void UpdateReservation(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        
    }
}
