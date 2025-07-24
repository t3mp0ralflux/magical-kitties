namespace MagicalKitties.Contracts.Requests.Endowments.Talents;

public class UpdateTalentRequest : UpdateEndowmentRequest
{
    public required bool IsPrimary { get; set; }
}