using MagicalKitties.Contracts.Responses.Characters;

namespace MagicalKitties.Contracts.Responses.Talents;

public class TalentResponse : EndowmentResponse
{
    public required bool IsPrimary { get; set; }
}