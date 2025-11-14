using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MilkingYield.API.Handlers;
using MilkingYield.API.HostedServices;

namespace MilkingYield.API.Extentions;

internal static class KafkaExtensions
{
    internal static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<IKafkaEventHandler, CattleDeletedEventHandler>();
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        // Kafka consumer registration
        services.AddSingleton(sp =>
        {
            KafkaSettings settings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
            var producerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
            };
            return new ConsumerBuilder<string, string>(producerConfig).Build();
        });

        // Kafka consumer background service (subscribes to the topic)
        services.AddHostedService<MilkingYieldConsumerBackgroundService>();
        return services;
    }
}
