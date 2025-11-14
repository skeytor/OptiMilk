
using MilkingYield.API.Events;
using MilkingYield.API.Services;
using System.Text.Json;

namespace MilkingYield.API.Handlers;

public sealed class CattleDeletedEventHandler(
    MilkingSessionService milkingSession,
    ILogger<CattleDeletedEventHandler> logger) : IKafkaEventHandler
{
    public string EventType => "CattleDeletedEvent";

    public async Task HandleAsync(string message, CancellationToken ct = default)
    {
        var evt = JsonSerializer.Deserialize<CattleDeletedEvent>(message);
        if (evt is not null)
        {
            await milkingSession.DeleteByCowIdAsync(evt.CattleId);
            logger.LogInformation("Handled CattleDeletedEvent for CattleId: {CattleId}", evt.CattleId);
        }
    }
}
