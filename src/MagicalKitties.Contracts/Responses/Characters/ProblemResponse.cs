namespace MagicalKitties.Contracts.Responses.Characters;

public class ProblemResponse
{
    public required int Id { get; set; }
    public required string Source { get; set; }
    public required string Emotion { get; set; }
    public required int Rank { get; set; }
    public required bool Solved { get; set; }
}