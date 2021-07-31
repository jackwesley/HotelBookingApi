using System;
using System.ComponentModel.DataAnnotations;

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

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CheckIn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CheckOut { get; set; }
    }
}
