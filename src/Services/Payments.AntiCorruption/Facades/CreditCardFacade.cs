using Payments.AntiCorruption.Gateways;
using Payments.Business;
using Payments.Business.Interfaces;
using System.Threading.Tasks;

namespace Payments.AntiCorruption.Facades
{
    public class CreditCardFacade : ICreditCardFacade
    {
        private readonly IPayPalGateway _payPalGateway;
        public CreditCardFacade(IPayPalGateway payPalGateway)
        {
            _payPalGateway = payPalGateway;
        }

        public async Task<string> Checkout(Order order, Payment payment)
        {
            return await _payPalGateway.CommitTransaction();
        }
    }
}