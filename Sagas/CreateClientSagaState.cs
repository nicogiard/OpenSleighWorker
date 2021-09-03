using System;
using OpenSleigh.Core;

namespace OpenSleighWorker.Sagas
{
    public class CreateClientSagaState : SagaState
    {
        public CreateClientSagaState(Guid id) : base(id) { }

        public enum Steps
        {
            Processing,
            Successful,
            Failed
        };
        public Steps CurrentStep { get; set; } = Steps.Processing;
    }
}