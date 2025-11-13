namespace CattleManagement.API.Events;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string AutoOffsetReset { get; set; } = string.Empty;
    public KafkaTopics Topics { get; set; } = null!;
}
public class KafkaTopics
{
    public string CattleEvents { get; set; } = string.Empty;
    public string MilkingEvents { get; set; } = string.Empty;
}
