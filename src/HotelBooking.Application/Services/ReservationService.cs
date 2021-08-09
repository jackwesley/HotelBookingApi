using HotelBooking.Application.DTOs;
using HotelBooking.Application.Mappers;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository reservationRepository, ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _logger = logger;
        }

        public async Task<ResponseResult<string>> CheckAvailabilityAsync(DateTime checkIn, DateTime checkOut)
        {
            try
            {
                return await AvailabilityForCheckinAndCheckoutAsync(checkIn, checkOut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to fetch check availability for the month");
                return ResponseResultFactory.CreateResponseServerError<string>("Error trying to fetch check availability for the month");
            }
        }

        private async Task<ResponseResult<string>> AvailabilityForCheckinAndCheckoutAsync(DateTime checkIn, DateTime checkOut)
        {
            var stayTime = new StayTime(checkIn, checkOut);

            if (!stayTime.IsValid())
            {
                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet<string>(HttpStatusCode.BadRequest, stayTime.ValidationResult, string.Empty);
            }

            if (await IsAvailableForDatesAsync(stayTime.DaysToStay))
            {
                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is available for CheckIn: {checkIn.ToString("yyyy-MM-dd")} and checkOut:{checkOut.ToString("yyyy-MM-dd")}");
            }

            return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {checkIn.ToString("yyyy-MM-dd")} and checkOut:{checkOut.ToString("yyyy-MM-dd")}");
        }

        public async Task<ResponseResult<ReservationDto>> PlaceReservationAsync(ReservationDto reservationDto)
        {
            try
            {
                return await CreateReservationHandleAsync(reservationDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to place Reservation");
                return ResponseResultFactory.CreateResponseServerError<ReservationDto>(null);
            }
        }

        private async Task<ResponseResult<ReservationDto>> CreateReservationHandleAsync(ReservationDto reservationDto)
        {
            var reservation = ReservationDtoMapper.ToReservation(reservationDto);

            if (!reservation.IsValid())
            {
                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet<ReservationDto>(HttpStatusCode.BadRequest, reservation.ValidationResult, null);
            }

            if (await IsAvailableForDatesAsync(reservation.StayTime.DaysToStay))
            {
                return await CreateReservationHandleAsync(reservation);
            }

            return ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.BadRequest, $"Room is NOT available for CheckIn: {reservationDto.CheckIn.ToString("yyyy-MM-dd")} and checkOut:{reservationDto.CheckOut.ToString("yyyy-MM-dd")}", null);
        }

        public async Task<ResponseResult<ReservationDto>> ModifyReservationAsync(UpdateReservationDto updateReservationDto)
        {
            try
            {
                return await ModifyReservationHandleAsync(updateReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to modify Reservation");
                return ResponseResultFactory.CreateResponseServerError<ReservationDto>(null);
            }
        }

        private async Task<ResponseResult<ReservationDto>> ModifyReservationHandleAsync(UpdateReservationDto updateReservationDto)
        {
            Reservation reservation = await _reservationRepository.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn);

            if (reservation == null)
            {
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.NotFound, "Reservation not found.", null);
            }

            reservation.StayTime.UpdateCheckin(updateReservationDto.NewCheckIn);
            reservation.StayTime.UpdateCheckout(updateReservationDto.NewCheckOut);

            if (!reservation.IsValid())
            {
                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet<ReservationDto>(HttpStatusCode.BadRequest, reservation.ValidationResult, null);
            }

            if (await IsAvailableForDatesAsync(reservation.StayTime.DaysToStay))
            {
                return await UpdateReservationAsync(reservation);
            }

            return ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.BadRequest, $"Room is NOT available for CheckIn: {updateReservationDto.NewCheckIn.ToString("yyyy-MM-dd")} and checkOut:{updateReservationDto.NewCheckOut.ToString("yyyy-MM-dd")}", null);
        }

        public async Task<ResponseResult<string>> CancelReservationAsync(Guid guestId, DateTime checkin)
        {
            try
            {
                var reservationToCancel = await _reservationRepository.GetByGuestIdAndCheckinAsync(guestId, checkin);

                if (reservationToCancel != null)
                {
                    return await CancelReservationHandleAsync(reservationToCancel);
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.NotFound, "Reservation not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to cancel Reservation");
                return ResponseResultFactory.CreateResponseServerError<string>("Reservation not canceled. Please contact administrators.");
            }
        }

        private async Task<ResponseResult<string>> CancelReservationHandleAsync(Reservation reservationToCancel)
        {
            if (await CancelReservationAsync(reservationToCancel))
            {
                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.NoContent, "Reservation canceled successfully.");
            }

            return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not canceled. Please contact administrators.", string.Empty);
        }

        private async Task<bool> IsAvailableForDatesAsync(List<DateTime> listDatesToCheck)
        {
            return await _reservationRepository.CheckAvailabilyForDatesAsync(listDatesToCheck);
        }

        private async Task<ResponseResult<ReservationDto>> CreateReservationHandleAsync(Reservation reservation)
        {
            if (await CreateReservationAsync(reservation))
            {
                var response = ReservationMapper.ToReservationDto(reservation);
                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.Created, response);
            }

            return ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.InternalServerError, "Reservation not saved. Please contact administrators.", null);
        }

        private async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            await _reservationRepository.AddReservationAsync(reservation);
            return await _reservationRepository.UnitOfWork.CommitAsync();
        }

        private async Task<ResponseResult<ReservationDto>> UpdateReservationAsync(Reservation reservation)
        {
            _reservationRepository.UpdateReservation(reservation);
            if(await _reservationRepository.UnitOfWork.CommitAsync())
            {
                var response = ReservationMapper.ToReservationDto(reservation);
                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, response);
            }

            return ResponseResultFactory.CreateResponseWithValidationResultNotSet<ReservationDto>(HttpStatusCode.InternalServerError, "Reservation not updated. Please contact administrators.", null);
        }

        private async Task<bool> CancelReservationAsync(Reservation reservation)
        {
            _reservationRepository.CancelReservation(reservation);
            return await _reservationRepository.UnitOfWork.CommitAsync();
        }
    }
}
