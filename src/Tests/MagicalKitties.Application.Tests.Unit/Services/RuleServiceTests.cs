using FluentAssertions;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Flaws;
using MagicalKitties.Application.Models.MagicalPowers;
using MagicalKitties.Application.Models.Rules;
using MagicalKitties.Application.Models.Talents;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class RuleServiceTests
{
    private readonly IFlawRepository _flawRepository = Substitute.For<IFlawRepository>();
    private readonly IMagicalPowerRepository _magicalPowerRepository = Substitute.For<IMagicalPowerRepository>();
    private readonly IProblemRepository _problemRepository = Substitute.For<IProblemRepository>();

    public readonly RuleService _sut;
    private readonly ITalentRepository _talentRepository = Substitute.For<ITalentRepository>();
    private readonly IUpgradeRepository _upgradeRepository = Substitute.For<IUpgradeRepository>();

    public RuleServiceTests()
    {
        _sut = new RuleService(_upgradeRepository, _flawRepository, _talentRepository, _magicalPowerRepository, _problemRepository);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCorrectPayload_WhenFunctionIsCalled()
    {
        // Arrange
        List<UpgradeRule> upgrades = Fakes.GenerateUpgradeRules();
        _upgradeRepository.GetRulesAsync().Returns(upgrades);

        List<Flaw> flaws = Fakes.GenerateFlaws();
        _flawRepository.GetAllAsync(Arg.Any<GetAllFlawsOptions>()).Returns(flaws);

        List<Talent> talents = Fakes.GenerateTalents();
        _talentRepository.GetAllAsync(Arg.Any<GetAllTalentsOptions>()).Returns(talents);

        List<MagicalPower> magicalPowers = Fakes.GenerateMagicalPower();
        _magicalPowerRepository.GetAllAsync(Arg.Any<GetAllMagicalPowersOptions>()).Returns(magicalPowers);

        List<ProblemRule> problemSources = Fakes.GenerateProblemSources();
        _problemRepository.GetAllProblemSourcesAsync().Returns(problemSources);

        List<ProblemRule> emotions = Fakes.GenerateEmotions();
        _problemRepository.GetAllEmotionsAsync().Returns(emotions);

        GameRules expectedResult = new()
                                   {
                                       MaxLevel = 10,
                                       MinAttributeValue = 0,
                                       MaxAttributeValue = 4,
                                       MinInjuries = 3,
                                       MaxInjuries = 4,
                                       LevelExperiencePoints = [0, 5, 6, 6, 7, 7, 8, 8, 9, 9],
                                       Attributes = ["Cunning, Cute, Fierce"],
                                       Flaws = flaws,
                                       Talents = talents,
                                       MagicalPowers = magicalPowers,
                                       Upgrades = upgrades,
                                       ProblemSources = problemSources,
                                       Emotions = emotions,
                                       DiceRules = ["+1 to +4 dice for Cute, Cunning, or Fierce", "+1 die for your Talent", "+2 dice for your Magical Power", "+1 die for an earlier success bonus", "-1 die per injury"],
                                       DiceDifficulties = DiceRule.DiceDifficulties,
                                       DiceSuccesses = DiceRule.DiceSuccesses,
                                       RollComplications = ["Foe or Disaster uses their Reaction.", "You suffer an Owie.", "You get into a sticky situation.", "You are unable to act for some time.", "You have one fewer die in your next dice pool.", "The GM forces you to take action according to your Flaw. (A Lazy kitty takes a nap, a Snobby kitty insults somebody important, or a Big-Mouthed kitty reveals crucial information.)", "A new Disaster is created.", "Something else bad happens (needs GM approval)."],
                                       RollBonus = ["A fellow kitty gains an extra die in their next dice pool.", "You or a fellow kitty shrug off one Owie you've suffered.", "You also accomplish a second goal.", "One Foe can't cause trouble for some time.", "Something else fun and exciting happens (this needs GM approval)."],
                                       RollSuperBonus = ["Your kitty and all your fellow kitties each gain an extra die to use in your next dice pools.", "You shrug off one injury you've suffered.", "You and all your fellow kitties shrug off one Owie you've suffered.", "One Foe suffers and extra Owie.", "You gain the extra effect of a Kitty Treat without needing to spend one.", "Something else super awesome happens (this needs GM approval)."],
                                       SpendingKittyTreats = ["Re-roll, any or all dice in your dice pool for one check.", "Avoid taking an Injury.", "Use a Bonus Feature you don't have for one of your Magical Powers, one time only.", "Add something to the story beyond your kitty's control."],
                                       Healing = ["-1 Injury at the end of the scene you suffered it.", "-1 Owie for one player on a success bonus.", "-1 Injury to yourself, or -1 Owie for everyone on a super success bonus.", "-1 to -2 Owies/Injuries using Healing Magical Power.", "-1 Owie using a Foe's Healing Reaction.", "Owies/Injuries reset to 0 at the episode's start (this needs GM approval)."],
                                       EndOfEpisodeInfo = "At the end of every episode, you and your fellow players should answer these questions as a group. For every question you answer \"yes\" to, make the adjustment noted.",
                                       EndEpisodeQuestions = ["Did the kitties save the day? (+1 XP)", "Did everybody have fun? (+1 XP)", "Did your kitty or their human learn a valuable lesson? (+1 XP)", "Did you fail on a roll? (+1 XP per fail)", "Did your human's or hometown's Problem get better or worse? (-3 to +3 ranks, usually -1)"]
                                   };

        // Act
        GameRules result = await _sut.GetAll(CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}