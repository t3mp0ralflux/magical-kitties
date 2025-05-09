namespace MagicalKitties.Contracts.Responses.Rules;

public class DiceSuccessResponse
{
    public required int Successes { get; set; }
    public required string Result { get; set; }
    public required string Enhancements { get; set; }
}