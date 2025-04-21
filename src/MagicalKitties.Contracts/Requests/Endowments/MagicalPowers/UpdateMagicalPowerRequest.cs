namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class UpdateMagicalPowerRequest : UpdateEndowmentRequest
{
    public required List<UpdateBonusFeatureRequest> BonusFeatures { get; set; }
}