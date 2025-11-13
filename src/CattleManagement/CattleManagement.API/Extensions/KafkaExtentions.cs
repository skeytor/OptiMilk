using CattleManagement.API.Events;
using Confluent.Kafka;

namespace CattleManagement.API.Extensions;

internal static class KafkaExtentions
{
    internal static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        var bootstrap = configuration["Kafka:BootstrapServers"]
                    ?? configuration.GetValue<string>("KAFKA:BOOTSTRAP_SERVERS")
                    ?? "localhost:9092";
        services.AddSingleton(sp =>
        {
            ProducerConfig config = new()
            {
                BootstrapServers = bootstrap,
            };
            return new ProducerBuilder<string, string>(config).Build();
        });
        services.AddSingleton<CattleKafkaProducerService>();
        return services;
    }
}
