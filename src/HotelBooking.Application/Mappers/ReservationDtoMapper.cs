using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Models;

namespace HotelBooking.Application.Mappers
{
    public static class ReservationDtoMapper
    {
        public static Reservation ToReservation(ReservationDto reservationDto)
        {
            var stayTime = new StayTime(reservationDto.CheckIn, reservationDto.CheckOut);
            return new Reservation(reservationDto.GuestId, stayTime);
        }
    }
}
