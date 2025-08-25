namespace MagicalKitties.Contracts.Requests.Characters;

public class UpgradeUpsertRequest
{
    public required Guid UpgradeId { get; init; }
    public required UpgradeOption UpgradeOption { get; init; }
    public required int Block { get; init; }
    public object? Value { get; init; }
}