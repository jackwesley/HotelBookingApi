using HotelBooking.Application.DTOs;
using HotelBooking.Core.DomainObjects;
using System;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services.Interfaces
{
    public interface IReservationService 
    {
        Task<ResponseResult> CheckAvailabilityForTheMonth();
        Task<ResponseResult> PlaceReservation(ReservationDto reservation);
        Task<ResponseResult> CancelReservation(Guid guestId, DateTime checkIn);
        Task<ResponseResult> ModifyReservation(Guid userId, DateTime oldCheckInDate, DateTime newCheckinDate, DateTime oldCheckOutDate, DateTime newCheckoutDate);

    }
}
