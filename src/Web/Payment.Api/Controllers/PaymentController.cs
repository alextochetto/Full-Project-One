using Architecture.Core.DomainObjects.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Business.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Payment.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", $"Usuário Autenticado: {User.Identity.IsAuthenticated}" };
        }

        [HttpPost]
        [Route("pay")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Pay([FromBody] OrderPayment orderPayment)
        {
            if (orderPayment is null)
                return BadRequest(orderPayment);

            var result = await _paymentService.Pay(orderPayment);

            if (result)
                return Ok(result);

            return BadRequest(orderPayment);
        }
    }
}