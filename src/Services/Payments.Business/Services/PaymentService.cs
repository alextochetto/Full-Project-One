using Architecture.Core.DomainObjects.DTO;
using Architecture.Core.DomainObjects.Enums;
using Payments.Business.Interfaces;
using System.Threading.Tasks;

namespace Payments.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICreditCardFacade _creditCardFacade;
        private readonly IDebitCardFacade _debitCardFacade;

        public PaymentService(ICreditCardFacade creditCardFacade, IDebitCardFacade debitCardFacade)
        {
            _creditCardFacade = creditCardFacade;
            _debitCardFacade = debitCardFacade;
        }

        public async Task<bool> Pay(OrderPayment orderPayment)
        {
            var order = new Order
            {
                Id = orderPayment.OrderId,
                Amount = orderPayment.OrderAmount
            };

            var payment = new Payment
            {
                OrderId = orderPayment.OrderId,
                Amount = orderPayment.OrderAmount,
                CardName = orderPayment.CardName,
                CardNumber = orderPayment.CardNumber,
                CardExpiredAt = orderPayment.CardExpiredAt,
                CardCvv = orderPayment.CardCvv,
                Status = Status.Processing
            };

            var transaction = string.Empty;
            switch (orderPayment.PaymentType)
            {
                case PaymentType.Credit:
                    transaction = await _creditCardFacade.Checkout(order, payment);
                    break;
                case PaymentType.Debit:
                    transaction = await _debitCardFacade.Checkout(order, payment);
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(transaction))
            {
                payment.Status = Status.Refused;
                return await Task.FromResult<bool>(false);
            }

            payment.Status = Status.Approved;
            return await Task.FromResult<bool>(true);
        }
    }
}