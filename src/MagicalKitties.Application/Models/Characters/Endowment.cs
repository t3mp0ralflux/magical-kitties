namespace MagicalKitties.Application.Models.Characters;

public class Endowment
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
    public virtual List<Endowment> BonusFeatures { get; set; } = [];
}