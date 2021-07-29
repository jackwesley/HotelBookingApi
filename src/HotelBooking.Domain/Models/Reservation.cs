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
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public Room Room { get; private set; }
        public virtual Guest Guest{get; set;}

        public Reservation(Guid guestId, Room room, DateTime checkIn, DateTime checkOut)
        {
            GuestId = guestId;
            Room = room;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public override bool IsValid()
        {
            ValidationResult = new ReservationValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class ReservationValidation : AbstractValidator<Reservation>
    {
        public ReservationValidation()
        {
            RuleFor(r => r.Room)
              .NotNull()
              .WithMessage("Room must be selected.");

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
