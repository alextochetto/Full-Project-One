using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Payment.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value1", "value2", $"Usuário Autenticado: {User.Identity.IsAuthenticated.ToString()}"};
        }
    }
}