using HotelBooking.Core.DomainObjects;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelBooking.Core.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponseHasErrors(ResponseResult responseResult)
        {
            return (responseResult.ValidationResult != null && responseResult.ValidationResult.Errors != null && responseResult.ValidationResult.Errors.Any());
        }

        protected ActionResult CustomResponse(ResponseResult result = null)
        {
            if (ResponseHasErrors(result))
                return NotFound(result.ValidationResult.Errors);

            return Ok(result.Response);
        }
    }
}
