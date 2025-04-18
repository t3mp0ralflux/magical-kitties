using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Models.Characters;

public class Character
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required string Username { get; init; }
    public required string Name { get; set; }
    public DateTime CreatedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
    public DateTime? DeletedUtc { get; set; }
    public string Description { get; set; } = "";
    public string Hometown { get; set; } = "";
    public List<Attribute> Attributes { get; set; } = [];

    public Flaw? Flaw { get; set; }

    public Talent? Talent { get; set; }

    // public List<Endowment> MagicalPowers { get; set; } = [];
    public Human? Human { get; set; }
    public int Level { get; set; } = 1;
    public int CurrentXp { get; set; }
    public int MaxOwies { get; set; }
    public int CurrentOwies { get; set; }
    public int StartingTreats { get; set; }
    public int CurrentTreats { get; set; }
    public int CurrentInjuries { get; set; }
}