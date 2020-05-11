using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Payments.Business.Interfaces
{
    public interface ICreditCardFacade
    {
        Task<string> Checkout(Order order, Payment payment);
    }
}