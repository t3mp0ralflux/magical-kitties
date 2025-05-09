namespace MagicalKitties.Application.Models.Characters;

public class UpgradeRule
{
    public Guid Id { get; set; }
    public int Block { get; set; }
    public Guid UpgradeChoice { get; set; }
}