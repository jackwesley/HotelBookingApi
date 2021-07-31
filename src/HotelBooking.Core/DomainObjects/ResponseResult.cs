using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;

namespace HotelBooking.Core.DomainObjects
{
    public class ResponseResult
    {
        public ResponseResult(object response, HttpStatusCode statusCode, ValidationResult validationResult)
        {
            Response = response;
            StatusCode = statusCode;
            ValidationResult = validationResult;
        }
        public object Response { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public IEnumerable<string> Errors => ValidationResult?.Errors.Select(x => x.ErrorMessage);


        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }
    }

    public static class ResponseResultFactory
    {
        public static ResponseResult CreateResponseWithValidationResultNotSet(HttpStatusCode httpStatusCode, string message)
        {
            var validationResult = new ValidationResult();
            var validationFailure = new ValidationFailure(string.Empty, message);
            validationResult.Errors.Add(validationFailure);

            return new ResponseResult(null, httpStatusCode, validationResult);
        }

        public static ResponseResult CreateResponseWithValidationResultAlreadySet(HttpStatusCode httpStatusCode, ValidationResult validationResult)
        {
            return new ResponseResult(null, httpStatusCode, validationResult);
        }

        public static ResponseResult CreateResponseResultSuccess(HttpStatusCode httpStatusCode, object response)
        {
            return new ResponseResult(response, httpStatusCode, null);
        }

        public static ResponseResult CreateResponseServerError()
        {
            return ResponseResultFactory.CreateResponseWithValidationResultNotSet(HttpStatusCode.InternalServerError, "Server error! Please contact administrators.");
        }
    }
}
