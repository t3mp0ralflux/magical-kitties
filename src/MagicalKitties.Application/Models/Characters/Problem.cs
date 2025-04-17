namespace MagicalKitties.Application.Models.Characters;

public class Problem
{
    public required int Id { get; set; }
    public required string Source { get; set; }
    public required Emotion Emotion { get; set; }
    public required int Rank { get; set; }
    public required bool Solved { get; set; }
}