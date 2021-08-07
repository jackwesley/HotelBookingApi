using FluentValidation;
using HotelBooking.Core.DomainObjects;
using System;
using System.Collections.Generic;

namespace HotelBooking.Domain.Models
{
    public class Reservation : Entity
    {
        //EF
        protected Reservation() { }

        public Guid GuestId { get; private set; }
        public Guid RoomId { get; private set; }
        public StayTime StayTime { get; private set; }
        public virtual Room Room { get; private set; }
        public virtual Guest Guest { get; private set; }

        public Reservation(Guid guestId, StayTime stayTime)
        {
            GuestId = guestId;
            RoomId = new Guid("539161dd-0ac5-4222-a410-24fbaf7dc70f");
            StayTime = stayTime;
        }

        public override bool IsValid()
        {
            StayTime.IsValid();
            ValidationResult = StayTime.ValidationResult;
            return ValidationResult.IsValid;
        }
    }
}
