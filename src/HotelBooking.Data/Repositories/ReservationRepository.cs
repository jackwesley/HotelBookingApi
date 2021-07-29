using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
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
                _context.Reservations.Add(reservation);
            
            else if (reservationExistent.CheckIn.Date == reservation.CheckIn.Date)
                return;
            else _context.Reservations.Add(reservation);
        }


        public async Task CancelReservation(Reservation reservation)
        {
            var reservationExistent = await GetByGuestId(reservation.GuestId);

            if (reservationExistent != null && reservationExistent.CheckIn.Date == reservation.CheckIn.Date)
                _context.Reservations.Remove(reservationExistent);
        }

        public async Task<Reservation> GetByCheckIn(DateTime checkIn)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.CheckIn == checkIn);
        }

        public async Task<Reservation> GetByGuestId(Guid guestId)
        {
            return await _context.Reservations.FirstOrDefaultAsync(x => x.GuestId.Equals(guestId));
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
