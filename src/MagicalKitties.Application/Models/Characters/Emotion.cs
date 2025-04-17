namespace MagicalKitties.Application.Models.Characters;

public class Emotion
{
    public Guid Id { get; set; }
    public int MinRange { get; set; }
    public int MaxRange { get; set; }
    public string Value { get; set; }
}