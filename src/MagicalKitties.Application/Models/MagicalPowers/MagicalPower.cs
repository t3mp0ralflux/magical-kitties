using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.MagicalPowers;

public class MagicalPower : Endowment
{
    [JsonPropertyName("is_primary")]
    [Column("is_primary")]
    public bool IsPrimary { get; init; }
    public List<MagicalPower> BonusFeatures { get; init; } = [];
}