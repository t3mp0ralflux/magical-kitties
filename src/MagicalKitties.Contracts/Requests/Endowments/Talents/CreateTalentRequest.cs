using MagicalKitties.Contracts.Requests.Endowments.Flaws;

namespace MagicalKitties.Contracts.Requests.Endowments.Talents;

public class CreateTalentRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public List<CreateFlawRequest> BonusFeatures { get; set; } = [];
}