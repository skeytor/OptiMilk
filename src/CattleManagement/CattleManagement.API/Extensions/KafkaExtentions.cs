using CattleManagement.API.HostedServices;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CattleManagement.API.Extensions;

internal static class KafkaExtentions
{
    internal static IServiceCollection AddKafkaProducer(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton(sp =>
        {
            KafkaSettings settings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
            ProducerConfig config = new()
            {
                BootstrapServers = settings.BootstrapServers,
            };
            return new ProducerBuilder<string, string>(config).Build();
        });
        services.AddSingleton<CattleKafkaProducerService>();
        return services;
    }
}
