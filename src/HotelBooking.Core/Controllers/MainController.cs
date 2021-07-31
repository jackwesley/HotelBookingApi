using HotelBooking.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
