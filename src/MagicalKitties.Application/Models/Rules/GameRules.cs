using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Models.Rules;

public class GameRules
{
    public required int MaxLevel { get; init; }
    public required int MinAttributeValue { get; init; }
    public required int MaxAttributeValue { get; init; }
    public required int MinInjuries { get; init; }
    public required int MaxInjuries { get; init; }
    public required int[] LevelExperiencePoints { get; init; } = [];
    public required List<string> Attributes { get; init; } = [];
    public required IEnumerable<Flaw> Flaws { get; init; }
    public required IEnumerable<Talent> Talents { get; init; }
    public required IEnumerable<MagicalPower> MagicalPowers { get; init; }
    public required IEnumerable<UpgradeRule> Upgrades { get; init; } = [];
    public required List<ProblemRule> ProblemSources { get; init; } = [];
    public required List<ProblemRule> Emotions { get; init; } = [];
    public required List<string> DiceRules { get; init; } = [];
    public required List<string> RollInstructions { get; set; } = [];
    public required List<DiceRule.DiceDifficulty> DiceDifficulties { get; init; } = [];
    public required List<DiceRule.DiceSuccess> DiceSuccesses { get; init; } = [];
    public required List<string> RollComplications { get; init; } = [];
    public required List<string> RollBonus { get; init; } = [];
    public required List<string> RollSuperBonus { get; init; } = [];
    public required List<string> SpendingKittyTreats { get; init; } = [];
    public required List<string> Healing { get; init; } = [];
    public required string EndOfEpisodeInfo { get; init; }
    public required List<string> EndEpisodeQuestions { get; init; } = [];
}