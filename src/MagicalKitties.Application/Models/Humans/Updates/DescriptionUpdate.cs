namespace MagicalKitties.Application.Models.Humans.Updates;

public class DescriptionUpdate
{
    public required DescriptionOption DescriptionOption { get; init; }
    public required Guid HumanId { get; init; }
    public required string? Name { get; init; }
    public required string? Description { get; init; }
}