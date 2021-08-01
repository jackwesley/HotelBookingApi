using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Models;

namespace HotelBooking.Application.Mappers
{
    public static class ReservationMapper
    {
        public static ReservationDto ToReservationDto(Reservation reservation)
        {
            var reservationDto = new ReservationDto(reservation.GuestId, reservation.CheckIn, reservation.CheckOut);
            reservationDto.SetRumNumber(reservation.Room.Number);
            return reservationDto;
        }
    }
}
