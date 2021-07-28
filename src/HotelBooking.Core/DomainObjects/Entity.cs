using FluentValidation.Results;
using System;



namespace HotelBooking.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public ValidationResult ValidationResult { get; set; }

        public abstract bool IsValid();
    }
}
