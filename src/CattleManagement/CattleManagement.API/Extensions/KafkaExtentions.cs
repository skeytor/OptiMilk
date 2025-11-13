using CattleManagement.API.Services;
using Confluent.Kafka;

namespace CattleManagement.API.Extensions;

internal static class KafkaExtentions
{
    internal static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var bootstrap = configuration.GetValue<string>("Kafka:BootstrapServers")
                    ?? configuration.GetValue<string>("KAFKA:BOOTSTRAP_SERVERS")
                    ?? "localhost:9092";
        services.AddSingleton(sp =>
        {
            ProducerConfig config = new()
            {
                BootstrapServers = bootstrap
            };
            return new ProducerBuilder<string, string>(config).Build();
        });
        services.AddSingleton<KafkaProducerService>();
        services.AddHostedService<KafkaConsumerBackgroundService>();
        return services;
    }
}
