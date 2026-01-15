using System.ComponentModel.DataAnnotations.Schema;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Models.Characters;

public class Character
{
    public required Guid Id { get; init; }
    public required Guid AccountId { get; init; }
    public required string Username { get; init; }
    public required string Name { get; set; }

    [Column("created_utc")]
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;

    [Column("updated_utc")]
    public DateTime UpdatedUtc { get; init; } = DateTime.UtcNow;

    [Column("deleted_utc")]
    public DateTime? DeletedUtc { get; init; }

    public string Description { get; init; } = "";
    public string Hometown { get; init; } = "";
    public Flaw? Flaw { get; set; }
    public List<Talent> Talents { get; set; } = [];

    public List<MagicalPower> MagicalPowers { get; set; } = [];
    public List<Human> Humans { get; set; } = [];
    public int Level { get; set; } = 1;

    [Column("current_xp")]
    public int CurrentXp { get; init; }

    [Column("max_owies")]
    public int MaxOwies { get; init; }

    [Column("current_owies")]
    public int CurrentOwies { get; set; }

    [Column("starting_treats")]
    public int StartingTreats { get; init; }

    [Column("current_treats")]
    public int CurrentTreats { get; set; }

    [Column("current_injuries")]
    public int CurrentInjuries { get; set; }
    public bool Incapacitated { get; init; }
    public int Cunning { get; set; }
    public int Cute { get; set; }
    public int Fierce { get; set; }
    public List<Upgrade> Upgrades { get; set; } = [];

    public Character CreateCopy()
    {
        Guid newCharacterId = Guid.NewGuid();
        List<Human> copiedHumans = [];

        foreach (Human human in this.Humans)
        {
            Guid newHumanId = Guid.NewGuid();
            copiedHumans.Add(new Human
                             {
                                 Id = newHumanId,
                                 CharacterId = newCharacterId,
                                 Name = human.Name,
                                 Description = human.Description,
                                 DeletedUtc = human.DeletedUtc,
                                 Problems = human.Problems.Select(x=> new Problem
                                                                      {
                                                                          Id = Guid.NewGuid(),
                                                                          HumanId = newHumanId,
                                                                          Source = x.Source,
                                                                          CustomSource = x.CustomSource,
                                                                          Emotion = x.Emotion,
                                                                          CustomEmotion = x.CustomEmotion,
                                                                          Rank = x.Rank,
                                                                          Solved = x.Solved
                                                                      }).ToList()
                             });
        }
        
        return new Character
               {
                   Id = newCharacterId,
                   AccountId = this.AccountId,
                   Cunning = this.Cunning,
                   CurrentInjuries = this.CurrentInjuries,
                   Incapacitated = false,
                   CurrentOwies = this.CurrentOwies,
                   StartingTreats = this.StartingTreats,
                   CurrentTreats = this.CurrentTreats,
                   CurrentXp = this.CurrentXp,
                   MaxOwies = this.MaxOwies,
                   Cute = this.Cute,
                   Fierce = this.Fierce,
                   Upgrades = this.Upgrades,
                   DeletedUtc = this.DeletedUtc,
                   Description = this.Description,
                   Flaw = this.Flaw,
                   Talents = this.Talents,
                   MagicalPowers = this.MagicalPowers,
                   Humans = copiedHumans,
                   Level = this.Level,
                   Hometown = this.Hometown,
                   Username = this.Username,
                   Name = $"{this.Name} - Copy",
               };
    }
}