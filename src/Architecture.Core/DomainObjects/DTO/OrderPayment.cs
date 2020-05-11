using Architecture.Core.DomainObjects.Enums;
using System;
using System.Linq.Expressions;

namespace Architecture.Core.DomainObjects.DTO
{
    public class OrderPayment
    {
        public Guid OrderId { get; set; }
        public decimal OrderAmount { get; set; }

        public PaymentType PaymentType { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public DateTime CardExpiredAt { get; set; }
        public string CardCvv { get; set; }
    }
}