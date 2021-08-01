using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Models;

namespace HotelBooking.Application.Mappers
{
    public static class ReservationMapper
    {
        public static ReservationDto ToReservationDto(Reservation reservation)
        {
            var reservationDto = new ReservationDto(reservation.GuestId, reservation.StayTime.CheckIn, reservation.StayTime.CheckOut);
            reservationDto.SetRumNumber(reservation.Room?.Number != null ? reservation.Room.Number : 1);
            return reservationDto;
        }
    }
}
