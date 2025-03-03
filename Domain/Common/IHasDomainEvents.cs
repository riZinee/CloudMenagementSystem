﻿namespace Domain.Common
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearEvents();
    }
}
