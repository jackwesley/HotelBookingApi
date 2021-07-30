using FluentValidation.Results;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Services.Interfaces;
using HotelBooking.Core.DomainObjects;
using HotelBooking.Domain.Models;
using HotelBooking.Domain.Repositories;
using System.Threading.Tasks;

namespace HotelBooking.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly IGuestRepository _guestRepository;

        public GuestService(IGuestRepository guestRepository)
        {
            _guestRepository = guestRepository;
        }

        public async Task<ResponseResult> CreateGuest(GuestDto guest)
        {
            ResponseResult result = new();
            var guestEntity = new Guest(guest.Name, guest.Document, guest.Email, guest.Phone);
           
            if (guestEntity.IsValid())
            {
                _guestRepository.AddGuest(guestEntity);
                await _guestRepository.UnitOfWork.Commit();
                result.Response = guest;
                return result;
            }

            result.ValidationResult = guestEntity.ValidationResult;

            return result;
        }

        public async Task<ResponseResult> GetGuestByEmail(string email)
        {
            var response = new ResponseResult();
            var guest = await _guestRepository.GetByEmail(email);

            if (guest != null)
            {
                response.Response = new GuestDto(guest.Id, guest.Name, guest.Document, guest.Email, guest.Phone);
                return response;
            }
            else
            {
                response.ValidationResult = new ValidationResult();
                var validationFailure = new ValidationFailure("email", "Guest not found!");
                response.ValidationResult.Errors.Add(validationFailure);

                return response;
            }

        }
    }
}
