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

        public ResponseResult CheckAvailability(DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var stayTime = new StayTime(checkIn, checkOut);

                if (!stayTime.IsValid())
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, stayTime.ValidationResult);
                }

                if (IsAvailableForDates(stayTime.DaysToStay))
                {
                    return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is available for CheckIn: {checkIn.ToString("yyyy-MM-dd")} and checkOut:{checkOut.ToString("yyyy-MM-dd")}");
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {checkIn.ToString("yyyy-MM-dd")} and checkOut:{checkOut.ToString("yyyy-MM-dd")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to fetch check availability for the month");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> PlaceReservationAsync(ReservationDto reservationDto)
        {
            try
            {
                var reservation = ReservationDtoMapper.ToReservation(reservationDto);

                if (!reservation.IsValid())
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, reservation.ValidationResult);
                }

                if (IsAvailableForDates(reservation.StayTime.DaysToStay))
                {
                    return await CreateReservationAsync(reservationDto);
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {reservationDto.CheckIn.ToString("yyyy-MM-dd")} and checkOut:{reservationDto.CheckOut.ToString("yyyy-MM-dd")}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to place Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> ModifyReservationAsync(UpdateReservationDto updateReservationDto)
        {
            try
            {
                Reservation reservation = await _reservationRepository.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn);

                if (reservation == null)
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.NotFound, "Reservation not found.");
                }

                reservation.StayTime.UpdateCheckin(updateReservationDto.NewCheckIn);
                reservation.StayTime.UpdateCheckout(updateReservationDto.NewCheckOut);

                if (!reservation.IsValid())
                    return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, reservation.ValidationResult);

                if (IsAvailableForDates(reservation.StayTime.DaysToStay))
                {
                    if (await UpdateReservationAsync(reservation))
                    {
                        var response = ReservationMapper.ToReservationDto(reservation);
                        return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, response);
                    }
                    else
                    {
                        return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not updated. Please contact administrators.");
                    }
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {updateReservationDto.NewCheckIn.ToString("yyyy-MM-dd")} and checkOut:{updateReservationDto.NewCheckOut.ToString("yyyy-MM-dd")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to modify Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> CancelReservationAsync(Guid guestId, DateTime checkin)
        {
            try
            {
                var reservationToCancel = await _reservationRepository.GetByGuestIdAndCheckinAsync(guestId, checkin);

                if (reservationToCancel != null)
                {
                    if (await CancelReservationAsync(reservationToCancel))
                    {
                        return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.NoContent, "Reservation canceled successfully.");
                    }
                    else
                    {
                        return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not canceled. Please contact administrators.");
                    }
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.NotFound, "Reservation not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to cancel Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        private bool IsAvailableForDates(List<DateTime> listDatesToCheck)
        {
            return _reservationRepository.CheckAvailabilyForDates(listDatesToCheck);
        }

        private async Task<ResponseResult> CreateReservationAsync(ReservationDto reservationDto)
        {
            var reservation = ReservationDtoMapper.ToReservation(reservationDto);

            if (reservation.IsValid())
            {
                if (await AddReservationAsync(reservation))
                {
                    var response = ReservationMapper.ToReservationDto(reservation);
                    return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.Created, response);
                }
                else
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not saved. Please contact administrators.");
                }
            }

            return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, reservation.ValidationResult);
        }

        private async Task<bool> AddReservationAsync(Reservation reservation)
        {
            await _reservationRepository.AddReservationAsync(reservation);
            return await _reservationRepository.UnitOfWork.CommitAsync();
        }

        private async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            _reservationRepository.UpdateReservation(reservation);
            return await _reservationRepository.UnitOfWork.CommitAsync();
        }

        private async Task<bool> CancelReservationAsync(Reservation reservation)
        {
            _reservationRepository.CancelReservation(reservation);
            return await _reservationRepository.UnitOfWork.CommitAsync();
        }
    }
}
