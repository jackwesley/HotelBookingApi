using HotelBooking.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IReservationService 
    {
        Task<bool> CheckAvailability(DateTime checkIn, DateTime checkOut);
        Task<ReservationDto> PlaceReservation(ReservationDto reservation);
        Task<bool> CancelReservation(ReservationDto reservation);
        Task<ReservationDto> ModifyReservation(ReservationDto reservation);

    }
}
