using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagicalKitties.Application.Models.Humans;

public class Problem
{
    public required Guid Id { get; init; }

    [JsonPropertyName("human_id")]
    [Column("human_id")]
    public required Guid HumanId { get; init; }

    public required string Source { get; init; }
    public required string Emotion { get; init; }
    public required int Rank { get; init; }
    public required bool Solved { get; init; }
}