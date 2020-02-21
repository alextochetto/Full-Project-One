using Payments.AntiCorruption.Gateways;
using Payments.AntiCorruption.Interfaces;
using Payments.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payments.AntiCorruption.Facades
{
    public class CreditCardFacade : ICreditCardFacade
    {
        private readonly IPayPalGateway _payPalGateway;
        public CreditCardFacade(IPayPalGateway payPalGateway)
        {
            _payPalGateway = payPalGateway;
        }

        public void Checkout(Order order, Payment payment)
        {

        }
    }
}