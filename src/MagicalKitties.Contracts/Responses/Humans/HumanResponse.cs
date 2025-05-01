namespace MagicalKitties.Contracts.Responses.Humans;

public class HumanResponse
{
    public required Guid Id { get; set; }
    public required Guid CharacterId { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public List<ProblemResponse> Problems { get; set; } = [];
}