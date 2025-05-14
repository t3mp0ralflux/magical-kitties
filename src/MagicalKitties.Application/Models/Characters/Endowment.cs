using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagicalKitties.Application.Models.Characters;

[Serializable]
public class Endowment
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    [JsonPropertyName("is_custom")]
    [Column("is_custom")]
    public required bool IsCustom { get; init; }
}