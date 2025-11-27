namespace MagicalKitties.Contracts.Responses.Rules;

public class ProblemSourceResponse
{
    public required Guid Id { get; set; }
    public required string RollValue { get; set; }
    public required string Source { get; set; }
    public required string CustomSource { get; set; }
}