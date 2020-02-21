using System;

namespace Architecture.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}