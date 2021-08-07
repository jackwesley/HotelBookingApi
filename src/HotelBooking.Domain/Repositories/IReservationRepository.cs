using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetAllCheckingForTheMonthAsync();
        Task<Reservation> GetByGuestIdAsync(Guid guestId);
        Task AddReservationAsync(Reservation reservation);
        void UpdateReservation(Reservation reservation);
        void CancelReservation(Reservation reservation);
        Task<IEnumerable<Reservation>> GetAllReservationsByGuestIdAsync(Guid guestId);
        Task<Reservation> GetByGuestIdAndCheckinAsync(Guid guestId, DateTime checkin);
        Task<bool> CheckAvailabilyForDatesAsync(List<DateTime> datesToCheck);

    }
}
