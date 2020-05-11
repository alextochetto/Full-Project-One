using System;
using System.Threading.Tasks;

namespace Payments.AntiCorruption.Gateways
{
    public class EloGateway : IEloGateway
    {
        public async Task<string> CommitTransaction()
        {
            var random = new Random().Next(2);

            if (random.Equals(2))
                return random.ToString();

            return null;
        }
    }
}