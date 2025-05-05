namespace MagicalKitties.Application.Models.Humans.Updates;

public class DescriptionUpdate
{
    public required DescriptionOption DescriptionOption { get; set; }
    public required Guid HumanId { get; set; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
}