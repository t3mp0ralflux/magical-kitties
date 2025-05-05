using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagicalKitties.Application.Models.Humans;

public class Human
{
    public required Guid Id { get; init; }
    
    [JsonPropertyName("character_id")]
    [Column("character_id")]
    public required Guid CharacterId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; } = "";
    public List<Problem> Problems { get; set; } = [];
}