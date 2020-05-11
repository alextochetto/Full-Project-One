using Architecture.Core.DomainObjects.DTO;
using System.Threading.Tasks;

namespace Payments.Business.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> Pay(OrderPayment orderPayment);
    }
}