using FluentValidation.Results;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<ResponseResult> CheckAvailabilityForTheMonth()
        {
            ResponseResult result = new();
            var reservations = await _reservationRepository.GetAllCheckingForTheMonth();

            if (reservations == null)
            {
                result.Response = new { Message = $"Room have no reservations for the next 30 days" };
                return result;
            }

            CreateResultForAllReservations(result, reservations);
            return result;
        }

        public async Task<ResponseResult> PlaceReservation(ReservationDto reservationDto)
        {
            ResponseResult responseResult = new();
            var reservation = ReservationDtoToReservation(reservationDto);

            if (reservation.IsValid())
            {
                await _reservationRepository.AddReservation(reservation);
                if (await _reservationRepository.UnitOfWork.Commit())
                {
                    responseResult.Response = ReservationToReservationDto(reservation);
                    return responseResult;
                }
                else
                {
                    CreateErrorResponse(responseResult, "Reservation not saved. Please contact administrators.");
                    return responseResult;
                }
            }

            responseResult.ValidationResult = reservation.ValidationResult;
            return responseResult;
        }

        public async Task<ResponseResult> CancelReservation(Guid guestId, DateTime checkin)
        {
            await _reservationRepository.CancelReservation(guestId, checkin);
            await _reservationRepository.UnitOfWork.Commit();

            return new ResponseResult
            {
                Response = "Reservation canceled successfuly"
            };
        }

        public async Task<ResponseResult> ModifyReservation(Guid guestId, DateTime oldCheckInDate, DateTime newCheckinDate, DateTime oldCheckOutDate, DateTime newCheckoutDate)
        {
            ResponseResult responseResult = new();
            
            var reservation = await _reservationRepository.GetByGuestIdAndCheckin(guestId, oldCheckInDate);
            if (reservation == null)
            {
                CreateErrorResponse(responseResult, "Reservation not found.");
                return responseResult;

            }

            var reservationWithCheckinEqualsNewCheckout = await _reservationRepository.GetByCheckin(newCheckinDate);
            if(reservationWithCheckinEqualsNewCheckout != null )
            {
                CreateErrorResponse(responseResult, $"Already exists a reservation with checkin date equals {newCheckoutDate}.");
                return responseResult;
            }

            reservation.UpdateCheckin(newCheckinDate);
            reservation.UpdateCheckout(newCheckoutDate);
            await _reservationRepository.UpdateReservation(reservation);
            
            if(await _reservationRepository.UnitOfWork.Commit())
            {
                responseResult.Response = reservation;
                return responseResult;
            }

            CreateErrorResponse(responseResult, $"Reservation not updated. Contact administrators.");
            return responseResult;
        }

        private static void CreateErrorResponse(ResponseResult responseResult, string message)
        {
            responseResult.ValidationResult = new ValidationResult();
            var validationFailure = new ValidationFailure(string.Empty, message);
            responseResult.ValidationResult.Errors.Add(validationFailure);
        }

        private Reservation ReservationDtoToReservation(ReservationDto reservationDto)
        {
            return new Reservation(reservationDto.GuestId, reservationDto.CheckIn, reservationDto.CheckOut);
        }

        private ReservationDto ReservationToReservationDto(Reservation reservation)
        {
            return new ReservationDto(reservation.GuestId, reservation.RoomId, reservation.CheckIn, reservation.CheckOut);
        }

        private static void CreateResultForAllReservations(ResponseResult result, IEnumerable<Reservation> reservations)
        {
            var reserved = new List<object>();

            foreach (var reservation in reservations)
            {
                reserved.Add(new { Checkin = reservation.CheckIn, Checkout = reservation.CheckOut });
            }

            result.Response = new
            {
                Message = $"Room not available for following Dates ",
                Reservations = reserved
            };
        }
    }
}
