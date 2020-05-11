using System.Threading.Tasks;

namespace Payments.AntiCorruption.Gateways
{
    public interface IPayPalGateway
    {
        Task<string> CommitTransaction();
    }
}