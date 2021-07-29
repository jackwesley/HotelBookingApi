using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class ReservationService : IReservationService
    {
        public Task<bool> CancelReservation(ReservationDto reservation)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckAvailability(DateTime checkIn, DateTime checkOut)
        {
            throw new NotImplementedException();
        }

        public Task<ReservationDto> ModifyReservation(ReservationDto reservation)
        {
            throw new NotImplementedException();
        }

        public Task<ReservationDto> PlaceReservation(ReservationDto reservation)
        {
            throw new NotImplementedException();
        }
    }
}
