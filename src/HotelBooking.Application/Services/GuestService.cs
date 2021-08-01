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

        public async Task<ResponseResult<GuestDto>> CreateGuestAsync(GuestDto guest)
        {
            var guestEntity = new Guest(guest.Name, guest.Document, guest.Email, guest.Phone);

            try
            {
                if (guestEntity.IsValid())
                {

                    if (await CreateGuestAsync(guestEntity))
                    {
                        guest.Id = guestEntity.Id;
                        return ResponseResultFactory.CreateResponseResultSuccess<GuestDto>(HttpStatusCode.Created, guest);
                    }

                    return ResponseResultFactory.CreateResponseWithValidationResultNotSet<GuestDto>(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.", null);
                }

                return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet<GuestDto>(HttpStatusCode.BadRequest, guestEntity.ValidationResult, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to create Guest");
                return ResponseResultFactory.CreateResponseServerError<GuestDto>(null);
            }
        }

        public async Task<ResponseResult<GuestDto>> GetGuestByEmailAsync(string email)
        {
            try
            {
                var guest = await _guestRepository.GetByEmailAsync(email);
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

                    return ResponseResultFactory.CreateResponseWithValidationResultAlreadySet<GuestDto>(HttpStatusCode.NotFound, validationResult, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to create Guest");
                return ResponseResultFactory.CreateResponseServerError<GuestDto>(null);
            }

        }

        private async Task<bool> CreateGuestAsync(Guest guest)
        {
            await _guestRepository.AddGuestAsync(guest);
            return await _guestRepository.UnitOfWork.CommitAsync();
        }


    }
}
