using HotelBooking.Core.Data;
using HotelBooking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Domain.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<Reservation> GetByCheckIn(DateTime checkIn);
        Task<Reservation> GetByGuestId(Guid guestId);
        Task AddReservation(Reservation reservation);
        Task UpdateReservation(Reservation reservation);
        Task CancelReservation(Reservation reservation);
    }
}
