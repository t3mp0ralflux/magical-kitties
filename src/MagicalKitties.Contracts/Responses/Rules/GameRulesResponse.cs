using MagicalKitties.Contracts.Responses.Flaws;
using MagicalKitties.Contracts.Responses.MagicalPowers;
using MagicalKitties.Contracts.Responses.Talents;

namespace MagicalKitties.Contracts.Responses.Rules;

public class GameRulesResponse
{
    public required int MaxLevel { get; set; }
    public required int MinAttributeValue { get; set; }
    public required int MaxAttributeValue { get; set; }
    public required int MinInjuries { get; set; }
    public required int MaxInjuries { get; set; }
    public required int[] LevelExperiencePoints { get; set; } = [];
    public required IEnumerable<string> Attributes { get; set; } = [];
    public required IEnumerable<FlawResponse> Flaws { get; set; }
    public required IEnumerable<TalentResponse> Talents { get; set; }
    public required IEnumerable<MagicalPowerResponse> MagicalPowers { get; set; }
    public required IEnumerable<UpgradeRuleResponse> Upgrades { get; set; } = [];
    public required IEnumerable<ProblemSourceResponse> ProblemSource { get; set; } = [];
    public required IEnumerable<ProblemSourceResponse> Emotion { get; set; } = [];
    public required IEnumerable<string> DiceRules { get; set; } = [];
    public required IEnumerable<string> RollInstructions { get; set; } = [];
    public required IEnumerable<DiceDifficultyResponse> DiceDifficulties { get; set; } = [];
    public required IEnumerable<DiceSuccessResponse> DiceSuccesses { get; set; } = [];
    public required IEnumerable<string> RollComplications { get; set; } = [];
    public required IEnumerable<string> RollBonus { get; set; } = [];
    public required IEnumerable<string> RollSuperBonus { get; set; } = [];
    public required IEnumerable<string> SpendingKittyTreats { get; set; } = [];
    public required IEnumerable<string> Healing { get; set; } = [];
    public required string EndOfEpisodeInfo { get; set; }
    public required IEnumerable<string> EndEpisodeQuestions { get; set; } = [];
}