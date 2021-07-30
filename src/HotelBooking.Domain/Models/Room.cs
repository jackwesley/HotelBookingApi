using HotelBooking.Core.DomainObjects;
using System;

namespace HotelBooking.Domain.Models
{
    public class Room : Entity
    {
        public int Number { get; private set; }

        public Room()
        {
            Id = new Guid("539161dd-0ac5-4222-a410-24fbaf7dc70f");
            Number = 1;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
