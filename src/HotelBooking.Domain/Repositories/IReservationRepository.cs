using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<IEnumerable<Reservation>> GetAllCheckingForTheMonth();
        Task<Reservation> GetByGuestId(Guid guestId);
        Task AddReservation(Reservation reservation);
        Task UpdateReservation(Reservation reservation);
        Task CancelReservation(Guid guestId, DateTime checkin);
        Task<IEnumerable<Reservation>> GetAllReservationsByGuestId(Guid guestId);
        Task<Reservation> GetByGuestIdAndCheckin(Guid guestId, DateTime checkin);
        Task<Reservation> GetByCheckin(DateTime checkin);
    }
}
