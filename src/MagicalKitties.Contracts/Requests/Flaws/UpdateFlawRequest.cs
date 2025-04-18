namespace MagicalKitties.Contracts.Requests.Flaws;

public class UpdateFlawRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public required List<UpdateFlawRequest> BonusFeatures { get; init; } = [];
}