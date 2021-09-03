using System;
using OpenSleigh.Core.Messaging;
using OpenSleighWorker.Dtos.In;

namespace OpenSleighWorker.Commands
{
    public record CreateClient(Guid Id, Guid CorrelationId, ClientDto data) : ICommand
    {
        public static CreateClient New(Guid correlationId, ClientDto data) => new(Guid.NewGuid(), correlationId, data);
    }
}