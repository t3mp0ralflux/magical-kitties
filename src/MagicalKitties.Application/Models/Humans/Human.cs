using System.ComponentModel.DataAnnotations.Schema;

namespace MagicalKitties.Application.Models.Humans;

public class Human
{
    public required Guid Id { get; init; }
    
    [Column("character_id")]
    public required Guid CharacterId { get; init; }
    public required string Name { get; init; }
    public string Description { get; init; } = "";
    public List<Problem> Problems { get; set; } = [];
}