using FluentAssertions;
using FluentValidation;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Models.Humans;
using MagicalKitties.Application.Models.Humans.Updates;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services.Implementation;
using MagicalKitties.Application.Validators.Humans;
using NSubstitute;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class HumanServiceTests
{
    private readonly IHumanRepository _humanRepository = Substitute.For<IHumanRepository>();
    private readonly IProblemRepository _problemRepository = Substitute.For<IProblemRepository>();
    private readonly IValidator<Human> _humanValidator = new HumanValidator();
    private readonly IValidator<GetAllHumansOptions> _optionsValidator = new GetAllHumansOptionsValidator();

    public HumanServiceTests()
    {
        _sut = new HumanService(_humanRepository, _humanValidator, _optionsValidator, _problemRepository);
    }
    
    public HumanService _sut { get; set; }

    [Fact]
    public async Task UpdateDescriptionAsync_ShouldReturnFalse_WhenHumanDoesNotExist()
    {
        // Arrange
        Account account = Fakes.GenerateAccount();
        Character character = Fakes.GenerateCharacter(account);
        Human human = Fakes.GenerateHuman(character.Id);
        
        DescriptionUpdate update = new DescriptionUpdate
                                   {
                                       DescriptionOption = DescriptionOption.name,
                                       CharacterId = character.Id,
                                       HumanId = human.Id,
                                       Name = "Test",
                                       Description = null
                                   };

        _humanRepository.ExistsByIdAsync(update.CharacterId, update.HumanId).Returns(false);
        
        // Act
        bool result = await _sut.UpdateDescriptionAsync(update);

        // Assert
        result.Should().BeFalse();
    }
}