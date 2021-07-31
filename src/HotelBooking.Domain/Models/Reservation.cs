using FluentValidation;
using HotelBooking.Core.DomainObjects;
using System;

namespace HotelBooking.Domain.Models
{
    public class Reservation : Entity
    {
        //EF
        protected Reservation() { }

        public Guid GuestId { get; private set; }
        public Guid RoomId { get; private set; }
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public virtual Room Room { get; private set; }
        public virtual Guest Guest { get; private set; }

        public Reservation(Guid guestId, DateTime checkIn, DateTime checkOut)
        {
            GuestId = guestId;
            RoomId = new Guid("539161dd-0ac5-4222-a410-24fbaf7dc70f");
            CheckIn = checkIn;
            CheckOut = checkOut.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public override bool IsValid()
        {
            ValidationResult = new ReservationValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public void UpdateCheckin(DateTime newCheckin)
        {
            CheckIn = newCheckin;
        }

        public void UpdateCheckout(DateTime newCheckout)
        {
            CheckOut = newCheckout.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
    }

    public class ReservationValidation : AbstractValidator<Reservation>
    {
        public ReservationValidation()
        {
            RuleFor(r => r.CheckIn)
              .LessThan(r => r.CheckOut)
              .WithMessage("Checkin can not be greater than Checkout.");

            RuleFor(r => r.CheckOut.Subtract(r.CheckIn).Days)
              .LessThanOrEqualTo(3)
              .WithMessage("Stay time can not be longer than 3 days.");

            RuleFor(r => r.CheckIn)
                .LessThan(DateTime.Now.AddDays(30))
                .WithMessage("It is not possible to make a reservation with more than 30 days in advance");

            RuleFor(r => r.CheckIn)
                .GreaterThan(DateTime.Now)
                .WithMessage("All reservertions must start at least in the next day.");
        }
    }
}
