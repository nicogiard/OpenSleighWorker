using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenSleigh.Core;
using OpenSleigh.Core.Messaging;
using OpenSleighWorker.Commands;
using OpenSleighWorker.Events;

namespace OpenSleighWorker.Sagas
{
    public class CreateClientSaga :
        Saga<CreateClientSagaState>,
        IStartedBy<CreateClient>
    {
        private readonly ILogger<CreateClientSaga> _logger;

        public CreateClientSaga(ILogger<CreateClientSaga> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(IMessageContext<CreateClient> context, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"processing create client '{context.Message.CorrelationId}'...");

            var rand = new Random();
            if (rand.Next(0, 2) == 1)
            { 
                throw new ApplicationException("Erreur lors de la création du client !!!");
            }

            var dataIn = context.Message.data;
            _logger.LogInformation($"-- FirstName client '{dataIn.FirstName}'");
            _logger.LogInformation($"-- LastName client '{dataIn.LastName}'");

            var dataOut = new Dtos.Out.ClientDto()
            {
                ClientId = Guid.NewGuid(),
                FirstName = dataIn.FirstName,
                LastName = dataIn.LastName
            };
            _logger.LogInformation($"-- Id client '{dataOut.ClientId}'");

            var message = CreateClientCompleted.New(context.Message.CorrelationId, dataOut);
            Publish(message);

            _logger.LogInformation($"create client '{context.Message.CorrelationId}' processed!");

            State.MarkAsCompleted();
        }
    }
}