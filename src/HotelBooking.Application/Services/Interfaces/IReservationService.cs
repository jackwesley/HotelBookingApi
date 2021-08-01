using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IReservationService
    {
        ResponseResult<string> CheckAvailability(DateTime checkIn, DateTime checkOut);
        Task<ResponseResult<ReservationDto>> PlaceReservationAsync(ReservationDto reservation);
        Task<ResponseResult<string>> CancelReservationAsync(Guid guestId, DateTime checkIn);
        Task<ResponseResult<ReservationDto>> ModifyReservationAsync(UpdateReservationDto updateReservationDto);

    }
}
