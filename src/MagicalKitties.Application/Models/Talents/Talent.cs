using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MagicalKitties.Application.Models.Characters;

namespace MagicalKitties.Application.Models.Talents;

public class Talent : Endowment
{
    [JsonPropertyName("is_primary")]
    [Column("is_primary")]
    public bool IsPrimary { get; init; }   
}