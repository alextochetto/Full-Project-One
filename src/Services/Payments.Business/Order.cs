using System;

namespace Payments.Business
{
    public class Order
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
    }
}