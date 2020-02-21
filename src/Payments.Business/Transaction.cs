using Architecture.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Payments.Business
{
    public class Transaction : Entity
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
    }
}