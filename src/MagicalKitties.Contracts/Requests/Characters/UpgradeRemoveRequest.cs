namespace MagicalKitties.Contracts.Requests.Characters;

public class UpgradeRemoveRequest
{
    public required Guid UpgradeId { get; init; }
    public required UpgradeOption UpgradeOption { get; init; } 
    public string? Value { get; init; }
}