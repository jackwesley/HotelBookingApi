using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace HotelBooking.Core.DomainObjects
{
    public class ResponseResult<T>
    {
        public ResponseResult(T response, HttpStatusCode statusCode, ValidationResult validationResult)
        {
            Response = response;
            StatusCode = statusCode;
            ValidationResult = validationResult;
        }

        public T Response { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public IEnumerable<string> Errors => ValidationResult?.Errors.Select(x => x.ErrorMessage);

        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }

    public static class ResponseResultFactory
    {
        public static ResponseResult<T> CreateResponseWithValidationResultNotSet<T>(HttpStatusCode httpStatusCode, string message,T response)
        {
            var validationResult = new ValidationResult();
            var validationFailure = new ValidationFailure(string.Empty, message);
            validationResult.Errors.Add(validationFailure);

            return new ResponseResult<T>(response, httpStatusCode, validationResult);
        }

        public static ResponseResult<T> CreateResponseWithValidationResultAlreadySet<T>(HttpStatusCode httpStatusCode, ValidationResult validationResult,T response)
        {
            return new ResponseResult<T>(response, httpStatusCode, validationResult);
        }

        public static ResponseResult<T> CreateResponseResultSuccess<T>(HttpStatusCode httpStatusCode, T response)
        {
            return new ResponseResult<T>(response, httpStatusCode, null);
        }

        public static ResponseResult<T> CreateResponseServerError<T>(T response)
        {
            return ResponseResultFactory.CreateResponseWithValidationResultNotSet<T>(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.", response);
        }
    }
}
