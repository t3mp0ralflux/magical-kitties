namespace MagicalKitties.Contracts.Requests.Humans;

public class HumanProblemUpdateRequest
{
    public required Guid HumanId { get; init; }
    public required Guid ProblemId { get; init; }
    public string? Source { get; init; }
    public string? Emotion { get; init; }
    public int? Rank { get; init; }
    public bool? Solved { get; init; }
}