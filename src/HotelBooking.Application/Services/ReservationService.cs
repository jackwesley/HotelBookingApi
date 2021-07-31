using HotelBooking.Application.DTOs;
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

        public async Task<ResponseResult> CheckAvailabilityForTheMonth()
        {
            try
            {
                var reservations = await _reservationRepository.GetAllCheckingForTheMonthAsync();

                if (reservations == null)
                {
                    var response = new { Message = $"Room have no reservations for the next 30 days" };
                    return new ResponseResult(response, HttpStatusCode.OK, null);
                }

                return CreateResultForAllReservations(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to fetch check availability for the month");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> PlaceReservation(ReservationDto reservationDto)
        {
            try
            {
                var reservation = ReservationDtoToReservation(reservationDto);

                if (reservation.IsValid())
                {
                    if (await AddReservation(reservation))
                    {
                        var response = ReservationToReservationDto(reservation);
                        return new ResponseResult(response, HttpStatusCode.OK, null);
                    }
                    else
                    {
                        return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not saved. Please contact administrators.");
                    }
                }

                return new ResponseResult(null, HttpStatusCode.BadRequest, reservation.ValidationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to place Reservation");
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
                    _reservationRepository.CancelReservation(reservationToCancel);
                    await _reservationRepository.UnitOfWork.Commit();
                }

                return new ResponseResult("Reservation canceled successfuly", HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to cancel Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }

        public async Task<ResponseResult> ModifyReservation(UpdateReservationDto updateReservationDto)
        {
            try
            {
                if (await CheckIfReservationExists(updateReservationDto.NewCheckinDate))
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.BadRequest, $"Already exists a reservation with Checkin: {updateReservationDto.NewCheckinDate}.");
                }

                if (await CheckIfReservationExists(updateReservationDto.NewCheckoutDate))
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.BadRequest, $"Already exists a reservation with checkin date equals {updateReservationDto.NewCheckoutDate}.");
                }

                return await UpdateReservation(updateReservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to modify Reservation");
                return ResponseResultFactory.CreateResponseServerError();
            }
        }
        private async Task<bool> CheckIfReservationExists(DateTime dateToCheck)
        {
            return await _reservationRepository.GetByCheckinAsync(dateToCheck) != null;
        }

        private async Task<ResponseResult> UpdateReservation(UpdateReservationDto updateReservationDto)
        {
            var reservation = await _reservationRepository.GetByGuestIdAndCheckinAsync(updateReservationDto.GuestId, updateReservationDto.CurrentCheckInDate);

            if (reservation == null)
            {
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.NotFound, "Reservation not found.");
            }

            reservation.UpdateCheckin(updateReservationDto.NewCheckinDate);
            reservation.UpdateCheckout(updateReservationDto.NewCheckoutDate);

            if (reservation.IsValid())
            {
                _reservationRepository.UpdateReservation(reservation);
                if (await _reservationRepository.UnitOfWork.Commit())
                {
                    var response = ReservationToReservationDto(reservation);
                    return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, response);
                }
                else
                {
                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Reservation not updated. Please contact administrators.");
                }
            }

            return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.OK, reservation.ValidationResult);
        }

        private Reservation ReservationDtoToReservation(ReservationDto reservationDto)
        {
            return new Reservation(reservationDto.GuestId, reservationDto.CheckIn, reservationDto.CheckOut);
        }

        private ReservationDto ReservationToReservationDto(Reservation reservation)
        {
            return new ReservationDto(reservation.GuestId, reservation.CheckIn, reservation.CheckOut);
        }

        private static ResponseResult CreateResultForAllReservations(IEnumerable<Reservation> reservations)
        {
            var reserved = new List<object>();

            foreach (var reservation in reservations)
            {
                reserved.Add(new { Checkin = reservation.CheckIn, Checkout = reservation.CheckOut });
            }

            var response = new
            {
                Message = $"Room not available for following Dates ",
                Reservations = reserved
            };

            return new ResponseResult(response, HttpStatusCode.OK, null);
        }

        private async Task<bool> AddReservation(Reservation reservation)
        {
            var reservationExistent = await _reservationRepository.GetByGuestIdAsync(reservation.GuestId);

            if (reservationExistent == null)
            {
                await _reservationRepository.AddReservationAsync(reservation);
            }
            else if (reservationExistent.CheckIn.Date == reservation.CheckIn.Date)
            {
                return true;
            }
            else
            {
                await _reservationRepository.AddReservationAsync(reservation);
            }

            return await _reservationRepository.UnitOfWork.Commit();
        }
    }
}
