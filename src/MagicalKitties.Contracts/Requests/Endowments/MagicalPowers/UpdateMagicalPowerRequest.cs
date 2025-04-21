namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class UpdateMagicalPowerRequest : UpdateEndowmentRequest
{
    public required List<UpdateEndowmentRequest> BonusFeatures { get; set; }
}