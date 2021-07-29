using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.DTOs
{
    public class ReservationDto
    {
        public ReservationDto(Guid guestId, DateTime checkIn, DateTime checkOut)
        {
            GuestId = guestId;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public Guid GuestId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
