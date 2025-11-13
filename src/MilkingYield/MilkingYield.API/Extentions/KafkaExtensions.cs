using Confluent.Kafka;
using MilkingYield.API.Services;

namespace MilkingYield.API.Extentions;

internal static class KafkaExtensions
{
    internal static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var bootstrap = configuration.GetValue<string>("Kafka:BootstrapServers")
                    ?? configuration.GetValue<string>("KAFKA:BOOTSTRAP_SERVERS")
                    ?? "localhost:9092";
        // Kafka producer registration
        services.AddSingleton(sp =>
        {
            var config = new ProducerConfig { BootstrapServers = bootstrap };
            return new ProducerBuilder<string, string>(config).Build();
        });
        services.AddSingleton<KafkaProducerService>();

        // Kafka consumer background service (subscribes to "test-topic")
        services.AddHostedService<KafkaConsumerBackgroundService>();
        return services;
    }
}
