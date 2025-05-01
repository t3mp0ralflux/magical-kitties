namespace MagicalKitties.Application.Models.Humans.Updates;

public class ProblemUpdate
{
    public ProblemOption ProblemOption { get; set; }
    public required Guid HumanId { get; set; }
    public required Guid ProblemId { get; set; }
    public required string? Source { get; set; }
    public required string? Emotion { get; set; }
    public required int? Rank { get; set; }
    public required bool? Solved { get; set; }
    
}