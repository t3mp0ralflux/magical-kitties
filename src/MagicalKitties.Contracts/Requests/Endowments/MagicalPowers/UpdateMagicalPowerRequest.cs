namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class UpdateMagicalPowerRequest : UpdateEndowmentRequest
{
    public List<UpdateMagicalPowerRequest> BonusFeatures { get; set; } = [];
}