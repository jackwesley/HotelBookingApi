using FluentValidation;
using HotelBooking.Core.DomainObjects;
using System;
using System.Collections.Generic;

namespace HotelBooking.Domain.Models
{
    public class StayTime : Entity
    {
        public StayTime(DateTime checkIn, DateTime checkOut)
        {
            CheckIn = checkIn;
            CheckOut = checkOut.AddHours(23).AddMinutes(59).AddSeconds(59);
            UpdateDaysToStay();
        }

        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public List<DateTime> DaysToStay { get; private set; }
        //ER Relation
        public virtual List<Reservation> Reservations { get; set; }
        public void UpdateCheckin(DateTime newCheckin)
        {
            CheckIn = newCheckin;
            UpdateDaysToStay();
        }

        public void UpdateCheckout(DateTime newCheckout)
        {
            CheckOut = newCheckout.AddHours(23).AddMinutes(59).AddSeconds(59);
            UpdateDaysToStay();
        }

        private List<DateTime> UpdateDaysToStay()
        {
            DaysToStay = new();
            var countDays = CheckOut.Subtract(CheckIn).Days;

            DaysToStay.Add(CheckIn);
            for (int day = 1; day <= countDays; day++)
            {
                DaysToStay.Add(CheckIn.AddDays(day).Date);
            }

            return DaysToStay;
        }

        public override bool IsValid()
        {
            ValidationResult = new StayTimeValidation().Validate(this);
            return ValidationResult.IsValid;
        }

    }

    public class StayTimeValidation : AbstractValidator<StayTime>
    {
        public StayTimeValidation()
        {
            RuleFor(r => r.CheckIn)
              .LessThan(r => r.CheckOut)
              .WithMessage("Checkin can not be greater than Checkout.");

            RuleFor(r => r.DaysToStay.Count)
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
