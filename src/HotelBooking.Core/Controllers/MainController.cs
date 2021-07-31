using HotelBooking.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;

namespace HotelBooking.Core.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponseHasErrors(ResponseResult responseResult)
        {
            return (responseResult.Errors != null &&  responseResult.Errors.Any());
        }

        protected IActionResult CustomResponse(ResponseResult result = null)
        {
            HttpStatusCode statusCode = result != null ? result.StatusCode : HttpStatusCode.InternalServerError;
            
            return StatusCode((int)statusCode, result);
        }
    }
}
