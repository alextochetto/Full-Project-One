using Payments.AntiCorruption.Gateways;
using Payments.Business;
using Payments.Business.Interfaces;
using System.Threading.Tasks;

namespace Payments.AntiCorruption.Facades
{
    public class DebitCardFacade : IDebitCardFacade
    {
        private readonly IEloGateway _eloGateway;

        public DebitCardFacade(IEloGateway eloGateway)
        {
            _eloGateway = eloGateway;
        }

        public async Task<string> Checkout(Order order, Payment payment)
        {
            return await _eloGateway.CommitTransaction();
        }
    }
}