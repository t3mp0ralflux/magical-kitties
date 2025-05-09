namespace MagicalKitties.Contracts.Requests.Characters;

public class UpgradeUpsertRequest
{
    public required Guid UpgradeId { get; set; }
    public required UpgradeOption UpgradeOption { get; set; }
    public required AttributeOption AttributeOption { get; set; }
    public required int Level { get; set; }
    public required object Value { get; set; }
}