namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class CreateMagicalPowerRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public List<CreateMagicalPowerRequest> BonusFeatures { get; set; } = [];
}