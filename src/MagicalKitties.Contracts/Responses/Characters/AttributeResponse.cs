namespace MagicalKitties.Contracts.Responses.Characters;

public class AttributeResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Value { get; set; }
}