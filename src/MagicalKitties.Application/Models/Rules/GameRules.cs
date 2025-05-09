using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Talents;

namespace MagicalKitties.Application.Models.Rules;

public class GameRules
{
    public required int MaxLevel { get; set; }
    public required int MinAttributeValue { get; set; }
    public required int MaxAttributeValue { get; set; }
    public required int MinInjuries { get; set; }
    public required int MaxInjuries { get; set; }
    public required int[] LevelExperiencePoints { get; set; } = [];
    public required List<string> Attributes { get; set; } = [];
    public required IEnumerable<Flaw> Flaws { get; set; }
    public required IEnumerable<Talent> Talents { get; set; }
    public required IEnumerable<MagicalPower> MagicalPowers { get; set; }
    public required IEnumerable<UpgradeRule> Upgrades { get; set; } = [];
    public required List<ProblemRule.Problem> ProblemSource { get; set; } = [];
    public required List<ProblemRule.Emotion> Emotion { get; set; } = [];
    public required List<string> DiceRules { get; set; } = [];
    public required List<DiceRule.DiceDifficulty> DiceDifficulties { get; set; } = [];
    public required List<DiceRule.DiceSuccess> DiceSuccesses { get; set; } = [];
    public required List<string> RollComplications { get; set; } = [];
    public required List<string> RollBonus { get; set; } = [];
    public required List<string> RollSuperBonus { get; set; } = [];
    public required List<string> SpendingKittyTreats { get; set; } = [];
    public required List<string> Healing { get; set; } = [];
    public required string EndOfEpisodeInfo { get; set; }
    public required List<string> EndEpisodeQuestions { get; set; } = [];
}