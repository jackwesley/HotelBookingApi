using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace HotelBooking.Application.DTOs
{
    public class GuestDto
    {
        public GuestDto(string name, string document, string email, string phone)
        {
            Name = name;
            Document = document;
            Email = email;
            Phone = phone;
        }

        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }
}
