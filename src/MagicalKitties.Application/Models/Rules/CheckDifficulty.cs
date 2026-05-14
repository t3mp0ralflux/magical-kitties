namespace MagicalKitties.Application.Models.Rules;

public class CheckDifficulty
{
    public required int Difficulty { get; init; }
    public required string DifficultyDescription { get; init; }
    public required string Cute { get; init; }
    public required string Cunning { get; init; }
    public required string Fierce { get; init; }
}