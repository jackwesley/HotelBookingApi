using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Models;

namespace HotelBooking.Application.Mappers
{
    public static class ReservationMapper
    {
        public static ReservationDto ToReservationDto(Reservation reservation)
        {
            return new ReservationDto(reservation.GuestId, reservation.CheckIn, reservation.CheckOut);
        }
    }
}
