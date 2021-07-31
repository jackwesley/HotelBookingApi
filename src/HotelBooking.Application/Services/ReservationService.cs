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
                (ResponseResult responseResult, List<DateTime> datesToCheck) = VerifyAllowedStayAndGetListOfDaysToCheck(checkIn, checkOut);

                if (responseResult != null) return responseResult;

                if (CheckAvailabilityForDates(datesToCheck))
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
                (ResponseResult responseResult, List<DateTime> datesToCheck) = VerifyAllowedStayAndGetListOfDaysToCheck(reservationDto.CheckIn, reservationDto.CheckOut);

                if (responseResult != null) return responseResult;

                if (CheckAvailabilityForDates(datesToCheck))
                {
                    return await CreateReservation(reservationDto);
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {reservationDto.CheckIn.ToString("yyyy-MM-dd")} and checkOut:{reservationDto.CheckOut.ToString("yyyy-MM-dd")}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to place Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> ModifyReservation(UpdateReservationDto updateReservationDto)
        {
            try
            {
                (ResponseResult responseResult, List<DateTime> datesToCheck) = VerifyAllowedStayAndGetListOfDaysToCheck(updateReservationDto.NewCheckIn, updateReservationDto.NewCheckOut);

                if (responseResult != null) return responseResult;

                if (CheckAvailabilityForDates(datesToCheck))
                {
                    return await UpdateReservation(updateReservationDto);
                }

                return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, $"Room is NOT available for CheckIn: {updateReservationDto.NewCheckIn.ToString("yyyy-MM-dd")} and checkOut:{updateReservationDto.NewCheckOut.ToString("yyyy-MM-dd")}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to modify Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> CancelReservation(Guid guestId, DateTime checkin)
        {
            try
            {
                var reservationToCancel = await _reservationRepository.GetByGuestIdAndCheckinAsync(guestId, checkin);

                if (reservationToCancel != null)
                {
                    if (await CancelReservation(reservationToCancel))
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

        private (ResponseResult, List<DateTime>) VerifyAllowedStayAndGetListOfDaysToCheck(DateTime checkIn, DateTime checkOut)
        {

            if (checkIn > checkOut)
                return (ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.BadRequest, "CheckOut must be greater than CheckIn."), null);

            var daysAllowedToStay = 3;
            var datesToCheck = GetListOfDaysToCheckAvailability(checkIn, checkOut);

            if (datesToCheck.Count > daysAllowedToStay)
                return (ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.BadRequest, "Stay time can not be longer than 3 days."), null);
            else
                return (null, datesToCheck);
        }

        private bool CheckAvailabilityForDates(List<DateTime> listDatesToCheck)
        {
            return _reservationRepository.CheckAvailabilyForDates(listDatesToCheck);
        }

        private async Task<ResponseResult> CreateReservation(ReservationDto reservationDto)
        {
            var reservation = ReservationDtoMapper.ToReservation(reservationDto);

            if (reservation.IsValid())
            {
                if (await AddReservation(reservation))
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

        private List<DateTime> GetListOfDaysToCheckAvailability(DateTime checkIn, DateTime checkOut)
        {
            List<DateTime> datesToCheck = new();
            var countDays = checkOut.Subtract(checkIn).Days;

            datesToCheck.Add(checkIn);
            for (int day = 1; day <= countDays; day++)
            {
                datesToCheck.Add(checkIn.AddDays(day).Date);
            }

            return datesToCheck;
        }

        private async Task<ResponseResult> UpdateReservation(UpdateReservationDto updateReservationDto)
        {

            var reservation = await _reservationRepository.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckIn);

            if (reservation == null)
            {
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.NotFound, "Reservation not found.");
            }

            reservation.UpdateCheckin(updateReservationDto.NewCheckIn);
            reservation.UpdateCheckout(updateReservationDto.NewCheckOut);

            if (reservation.IsValid())
            {
                if (await UpdateReservation(reservation))
                {
                    var response = ReservationMapper.ToReservationDto(reservation);
                    return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, response);
                }
                else
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not updated. Please contact administrators.");
                }
            }

            return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.OK, reservation.ValidationResult);
        }

        private async Task<bool> AddReservation(Reservation reservation)
        {
            await _reservationRepository.AddReservationAsync(reservation);
            return await _reservationRepository.UnitOfWork.Commit();
        }

        private async Task<bool> UpdateReservation(Reservation reservation)
        {
            _reservationRepository.UpdateReservation(reservation);
            return await _reservationRepository.UnitOfWork.Commit();
        }

        private async Task<bool> CancelReservation(Reservation reservation)
        {
            _reservationRepository.CancelReservation(reservation);
            return await _reservationRepository.UnitOfWork.Commit();
        }
    }
}
