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

        public async Task AddReservation(Reservation reservation)
        {
            var reservationExistent = await GetByGuestId(reservation.GuestId);

            if (reservationExistent == null)
                await _context.Reservations.AddAsync(reservation);

            else if (reservationExistent.CheckIn.Date == reservation.CheckIn.Date)
                return;
            else await _context.Reservations.AddAsync(reservation);
        }

        public async Task CancelReservation(Guid guestId, DateTime checkin)
        {
            var reservations = await GetAllReservationsByGuestId(guestId);

            if (reservations != null)
            {
                var reservationToCancel = reservations.Where(x => x.CheckIn == checkin).FirstOrDefault();
                _context.Reservations.Remove(reservationToCancel);
            }
        }

        public async Task<IEnumerable<Reservation>> GetAllCheckingForTheMonth()
        {
            return await _context.Reservations
                .AsNoTracking().Where(x => x.CheckIn > DateTime.Now.AddDays(1) && x.CheckOut <= DateTime.Now.AddDays(30))
                .ToListAsync();
        }

        public async Task<Reservation> GetByGuestId(Guid guestId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.GuestId.Equals(guestId));
        }

        public async Task<Reservation> GetByGuestIdAndCheckin(Guid guestId, DateTime checkin)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.GuestId.Equals(guestId) && x.CheckIn == checkin);
        }

        public async Task<Reservation> GetByCheckin(DateTime checkin)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.CheckIn == checkin);
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsByGuestId(Guid guestId)
        {
            return await _context.Reservations
              .AsNoTracking().Where(x => x.GuestId == guestId)
              .ToListAsync(); ;
        }

        public async Task UpdateReservation(Reservation reservation)
        {
            var reservationExistent = await GetByGuestId(reservation.GuestId);
            if (reservationExistent != null && reservationExistent.CheckIn.Date == reservation.CheckIn.Date)
                _context.Reservations.Update(reservation);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
