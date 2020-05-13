using System;
using System.Threading.Tasks;

namespace Payments.AntiCorruption.Gateways
{
    public class PayPalGateway : IPayPalGateway
    {
        public async Task<string> CommitTransaction()
        {
            var random = new Random().Next(3);

            if (random.Equals(2))
                return random.ToString();

            return null;
        }
    }
}