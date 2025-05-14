namespace MagicalKitties.Contracts.Responses.Rules;

public class DiceDifficultyResponse
{
    public required int Difficulty { get; set; }
    public required string DifficultyText { get; set; }
    public required string CuteAction { get; set; }
    public required string CunningAction { get; set; }
    public required string FierceAction { get; set; }
}