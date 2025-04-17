namespace MagicalKitties.Contracts.Responses.Characters;

public class EndowmentResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool IsCustom { get; set; }
    public required List<EndowmentResponse> BonusFeatures { get; set; } = [];
}