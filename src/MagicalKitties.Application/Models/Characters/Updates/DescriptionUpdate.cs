namespace MagicalKitties.Application.Models.Characters.Updates;

public class DescriptionUpdate
{
    public DescriptionOption DescriptionOption { get; init; }
    public required Guid CharacterId { get; init; }
    public required Guid AccountId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Hometown { get; init; }
    public int? XP { get; init; }
}