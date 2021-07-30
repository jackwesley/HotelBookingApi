using FluentValidation.Results;
using System;
using System.Text.Json.Serialization;

namespace HotelBooking.Application.DTOs
{
    public class GuestDto
    {
        public GuestDto(Guid id, string name, string document, string email, string phone)
        {
            Id = id;
            Name = name;
            Document = document;
            Email = email;
            Phone = phone;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }
}
