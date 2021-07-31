using FluentValidation.Results;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;
        private readonly ILogger<GuestService> _logger;

        public GuestService(IGuestRepository guestRepository, ILogger<GuestService> logger)
        {
            _guestRepository = guestRepository;
            _logger = logger;
        }

        public async Task<ResponseResult> CreateGuest(GuestDto guest)
        {

            var guestEntity = new Guest(guest.Name, guest.Document, guest.Email, guest.Phone);

            try
            {
                if (guestEntity.IsValid())
                {
                    await _guestRepository.AddGuest(guestEntity);
                    await _guestRepository.UnitOfWork.Commit();

                    return ResponseResultFactory.CreateResponseResultSuccess (HttpStatusCode.OK, guest);
                }

                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, guestEntity.ValidationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to create Guest");
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.");
            }
            
        }

        public async Task<ResponseResult> GetGuestByEmail(string email)
        {
            var guest = await _guestRepository.GetByEmail(email);

            try
            {
                if (guest != null)
                {
                    var guestDto = new GuestDto(guest.Id, guest.Name, guest.Document, guest.Email, guest.Phone);
                    return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.OK, guestDto);
                }
                else
                {
                    var validationResult = new ValidationResult();
                    var validationFailure = new ValidationFailure("email", "Guest not found!");
                    validationResult.Errors.Add(validationFailure);

                    return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.NotFound, validationResult);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error trying to get Guest");
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.");
            }
           
        }

        
    }
}
