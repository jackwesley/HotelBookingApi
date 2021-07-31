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

        public async Task<ResponseResult> CreateGuestAsync(GuestDto guest)
        {
            var guestEntity = new Guest(guest.Name, guest.Document, guest.Email, guest.Phone);
            
            try
            {
                if (guestEntity.IsValid())
                {

                    if (await CreateGuestAsync(guestEntity))
                    {
                        guest.Id = guestEntity.Id;
                        return ResponseResultFactory.CreateResponseResultSuccess(HttpStatusCode.Created, guest);
                    }

                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.");
                }

                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet(HttpStatusCode.BadRequest, guestEntity.ValidationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to create Guest");
                return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.");
            }
        }

        public async Task<ResponseResult> GetGuestByEmailAsync(string email)
        {
            var guest = await _guestRepository.GetByEmailAsync(email);

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

        private async Task<bool> CreateGuestAsync(Guest guest)
        {
            await _guestRepository.AddGuestAsync(guest);
            return await _guestRepository.UnitOfWork.CommitAsync();
        }

        
    }
}
