namespace MagicalKitties.Contracts.Requests.Endowments;

public class CreateEndowmentRequest
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsCustom { get; init; }
}