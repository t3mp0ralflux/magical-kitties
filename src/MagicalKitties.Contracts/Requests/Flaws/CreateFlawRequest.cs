namespace MagicalKitties.Contracts.Requests.Flaws;

public class CreateFlawRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public required List<CreateFlawRequest> BonusFeatures { get; init; }
}