using HotelBooking.Core.DomainObjects;
using System;

namespace HotelBooking.Domain.Models
{
    public class Room : Entity
    {
        public int Number { get; private set; }

        public Room()
        {
            Number = 1;
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
