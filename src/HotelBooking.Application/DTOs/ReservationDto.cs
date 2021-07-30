using System;

namespace HotelBooking.Application.DTOs
{
    public class ReservationDto
    {
        public ReservationDto(Guid guestId, Guid room, DateTime checkIn, DateTime checkOut)
        {
            GuestId = guestId;
            Room = room;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public Guid GuestId { get; set; }
        public Guid Room { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
