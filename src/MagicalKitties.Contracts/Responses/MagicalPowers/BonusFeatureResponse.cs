namespace MagicalKitties.Contracts.Responses.MagicalPowers;

public class BonusFeatureResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public required bool Selected { get; set; }
}