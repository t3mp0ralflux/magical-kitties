namespace MagicalKitties.Contracts.Responses.Rules;

public class CheckDifficultyResponse
{
    public required int Difficulty { get; set; }
    public required string DifficultyDescription { get; set; }
    public required string Cute { get; set; }
    public required string Cunning { get; set; }
    public required string Fierce { get; set; }
}