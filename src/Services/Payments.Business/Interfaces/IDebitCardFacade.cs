using System.Threading.Tasks;

namespace Payments.Business.Interfaces
{
    public interface IDebitCardFacade
    {
        Task<string> Checkout(Order order, Payment payment);
    }
}
