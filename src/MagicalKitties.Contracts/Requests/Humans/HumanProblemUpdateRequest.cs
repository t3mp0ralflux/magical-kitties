namespace MagicalKitties.Contracts.Requests.Humans;

public class HumanProblemUpdateRequest
{
    public required Guid CharacterId { get; set; }
    public required Guid ProblemId { get; set; }
    public required string Source { get; set; }
    public required string Emotion { get; set; }
    public required int Rank { get; set; }
    public required bool Solved { get; set; }
}