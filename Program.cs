using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenSleigh.Core.DependencyInjection;
using OpenSleigh.Persistence.SQL;
using OpenSleigh.Persistence.SQLServer;
using OpenSleigh.Transport.Kafka;
using OpenSleighWorker.Commands;
using OpenSleighWorker.Events;
using OpenSleighWorker.Sagas;

namespace OpenSleighWorker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);
            var host = hostBuilder.Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(cfg => { cfg.AddConsole(); })
                        .AddOpenSleigh(cfg =>
                        {
                            var sqlConnStr = hostContext.Configuration.GetConnectionString("sql");
                            var sqlConfig = new SqlConfiguration(sqlConnStr);

                            var kafkaConnStr = hostContext.Configuration.GetConnectionString("Kafka");
                            var kafkaCfg = new KafkaConfiguration(kafkaConnStr);

                            cfg.UseKafkaTransport(kafkaCfg, builder =>
                            {
                                builder.UseMessageNamingPolicy<CreateClient>(() => new QueueReferences("command_create_client", "command_create_client.dead"));
                                builder.UseMessageNamingPolicy<CreateClientCompleted>(() => new QueueReferences("event_client_created", "event_client_created.dead"));
                            });
                            cfg.UseSqlServerPersistence(sqlConfig);

                            cfg.AddSaga<CreateClientSaga, CreateClientSagaState>()
                                .UseStateFactory<CreateClient>(msg => new CreateClientSagaState(msg.CorrelationId))
                                .UseKafkaTransport()
                                .UseRetryPolicy<CreateClient>(builder =>
                                {
                                    builder.WithMaxRetries(5)
                                        .Handle<ApplicationException>()
                                        .WithDelay(executionIndex => TimeSpan.FromSeconds(executionIndex))
                                        .OnException(ctx =>
                                        {
                                            Console.WriteLine(
                                                $"tentative #{ctx.ExecutionIndex} failed: {ctx.Exception.Message}");
                                        });
                                });
                        });
                });
    }
}