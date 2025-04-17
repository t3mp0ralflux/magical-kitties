namespace MagicalKitties.Application.Models.Characters;

public class Attribute
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Value { get; set; }
}