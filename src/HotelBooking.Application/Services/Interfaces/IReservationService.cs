using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IReservationService
    {
        ResponseResult CheckAvailability(DateTime checkIn, DateTime checkOut);
        Task<ResponseResult> PlaceReservationAsync(ReservationDto reservation);
        Task<ResponseResult> CancelReservation(Guid guestId, DateTime checkIn);
        Task<ResponseResult> ModifyReservation(UpdateReservationDto updateReservationDto);

    }
}
