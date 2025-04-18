namespace MagicalKitties.Contracts.Requests.Endowments.Flaws;

public class CreateFlawRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public List<CreateFlawRequest> BonusFeatures { get; set; } = [];
}