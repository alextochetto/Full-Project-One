using System;

namespace Payments.Business
{
    public class Payment
    {
        public Guid OrderId { get; set; }
        public Status Status { get; set; }
        public decimal Amount { get; set; }

        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public DateTime CardExpiredAt { get; set; }
        public string CardCvv { get; set; }
    }
}