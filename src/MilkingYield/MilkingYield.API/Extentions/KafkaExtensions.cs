using Confluent.Kafka;
using MilkingYield.API.Events;
using MilkingYield.API.Services;

namespace MilkingYield.API.Extentions;

internal static class KafkaExtensions
{
    internal static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Kafka consumer registration
        services.AddSingleton(sp =>
        {
            var kafkaSettings = sp.GetRequiredService<KafkaSettings>();
            var producerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                GroupId = kafkaSettings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
            };
            return new ConsumerBuilder<string, string>(producerConfig).Build();
        });

        // Kafka consumer background service (subscribes to "test-topic")
        services.AddHostedService<MilkingYieldConsumerBackgroundService>();
        return services;
    }
}
