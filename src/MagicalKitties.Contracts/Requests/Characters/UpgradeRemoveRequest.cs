namespace MagicalKitties.Contracts.Requests.Characters;

public class UpgradeRemoveRequest
{
    public required Guid UpgradeId { get; set; }
    public required UpgradeOption UpgradeOption { get; set; }
    public required object Value { get; set; }
}