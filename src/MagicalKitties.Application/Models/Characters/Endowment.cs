namespace MagicalKitties.Application.Models.Characters;

public class Endowment
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool IsCustom { get; set; }
    public required List<Endowment> BonusFeatures { get; set; } = [];
}