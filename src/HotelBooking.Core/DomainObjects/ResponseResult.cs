using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace HotelBooking.Core.DomainObjects
{
    public class ResponseResult
    {
        public object Response { get; set; }

        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }

    }
}
