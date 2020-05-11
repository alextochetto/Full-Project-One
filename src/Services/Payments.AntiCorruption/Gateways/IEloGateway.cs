using System.Threading.Tasks;

namespace Payments.AntiCorruption.Gateways
{
    public interface IEloGateway
    {
        Task<string> CommitTransaction();
    }
}