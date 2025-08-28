namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class UpdateMagicalPowerRequest : UpdateEndowmentRequest
{
    public required bool IsPrimary { get; set; }
    public List<UpdateMagicalPowerRequest> BonusFeatures { get; set; } = [];
}