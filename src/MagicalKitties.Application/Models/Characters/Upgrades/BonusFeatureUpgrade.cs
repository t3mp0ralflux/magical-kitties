namespace MagicalKitties.Application.Models.Characters.Upgrades;

public class BonusFeatureUpgrade
{
    public int MagicalPowerId { get; set; }
    public int? NestedMagicalPowerId { get; set; }
    public int BonusFeatureId { get; set; }
    public int? NestedBonusFeatureId { get; set; }
    public bool IsNested { get; set; }
}