using System;
using OpenSleigh.Core.Messaging;
using OpenSleighWorker.Dtos.Out;

namespace OpenSleighWorker.Events
{
    public record CreateClientCompleted(Guid Id, Guid CorrelationId, ClientDto data) : IEvent
    {
        public static CreateClientCompleted New(Guid correlationId, ClientDto data) => new(Guid.NewGuid(), correlationId, data);
    }
}