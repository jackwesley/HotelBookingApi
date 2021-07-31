using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Models;

namespace HotelBooking.Application.Mappers
{
    public static class ReservationDtoMapper
    {
        public static Reservation ToReservation(ReservationDto reservationDto)
        {
            return new Reservation(reservationDto.GuestId, reservationDto.CheckIn, reservationDto.CheckOut);
        }
    }
}
