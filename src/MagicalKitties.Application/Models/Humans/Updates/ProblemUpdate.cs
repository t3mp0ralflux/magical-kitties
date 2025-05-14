namespace MagicalKitties.Application.Models.Humans.Updates;

public class ProblemUpdate
{
    public ProblemOption ProblemOption { get; init; }
    public required Guid HumanId { get; init; }
    public required Guid ProblemId { get; init; }
    public required string? Source { get; init; }
    public required string? Emotion { get; init; }
    public required int? Rank { get; init; }
    public required bool? Solved { get; init; }
}