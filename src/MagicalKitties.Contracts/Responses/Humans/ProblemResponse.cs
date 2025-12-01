namespace MagicalKitties.Contracts.Responses.Humans;

public class ProblemResponse
{
    public required Guid Id { get; set; }
    public required Guid HumanId { get; set; }
    public required string Source { get; set; }
    public required string? CustomSource { get; set; }
    public required string Emotion { get; set; }
    public required string? CustomEmotion { get; set; }
    public required int Rank { get; set; }
    public required bool Solved { get; set; }
}