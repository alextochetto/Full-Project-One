using Architecture.Core.DomainObjects;
using System;

namespace Payments.Business
{
    public class Transaction : Entity
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
    }
}