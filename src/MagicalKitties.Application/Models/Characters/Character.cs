using System.ComponentModel.DataAnnotations.Schema;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Models.Characters;

public class Character
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required string Username { get; init; }
    public required string Name { get; set; }
    
    [Column("created_utc")]
    public DateTime CreatedUtc { get; init; }
    
    [Column("updated_utc")]
    public DateTime UpdatedUtc { get; init; }
    
    [Column("deleted_utc")]
    public DateTime? DeletedUtc { get; set; }
    public string Description { get; set; } = "";
    public string Hometown { get; set; } = "";
    public List<Attribute> Attributes { get; set; } = [];
    public Flaw? Flaw { get; set; }
    public Talent? Talent { get; set; }

    // public List<Endowment> MagicalPowers { get; set; } = [];
    public Human? Human { get; set; }
    public int Level { get; set; } = 1;
    
    [Column("current_xp")]
    public int CurrentXp { get; set; }
    
    [Column("max_owies")]
    public int MaxOwies { get; set; }
    
    [Column("current_owies")]
    public int CurrentOwies { get; set; }
    
    [Column("starting_treats")]
    public int StartingTreats { get; set; }
    
    [Column("current_treats")]
    public int CurrentTreats { get; set; }
    
    [Column("current_injuries")]
    public int CurrentInjuries { get; set; }
}