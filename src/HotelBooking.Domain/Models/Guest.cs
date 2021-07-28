using FluentValidation;
using HotelBooking.Core.DomainObjects;

namespace HotelBooking.Domain.Models
{
    public class Guest : Entity
    {
        //EF
        protected Guest() { }
        public Guest(string name, string document, string email, string phone)
        {
            Name = name;
            Document = document;
            Email = email;
            Phone = phone;
        }

        public string Name { get; private set; }
        public string Document { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public virtual Reservation Reservation { get; set; }

        public override bool IsValid()
        {
            ValidationResult = new GuestValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class GuestValidation : AbstractValidator<Guest>
    {
        public GuestValidation()
        {
            RuleFor(r => r.Name)
             .NotNull()
             .WithMessage("Name must not be null");

            RuleFor(r => r.Document)
             .NotNull()
             .WithMessage("Name must not be null");

            RuleFor(r => r.Email)
             .NotNull()
             .WithMessage("Name must not be null");

            RuleFor(r => r.Phone)
             .NotNull()
             .WithMessage("Name must not be null");
        }
    }
}