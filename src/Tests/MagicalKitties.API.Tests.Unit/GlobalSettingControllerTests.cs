using FluentAssertions;
using FluentValidation;
using MagicalKitties.Api.Controllers;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Application.Models.GlobalSettings;
using MagicalKitties.Application.Services;
using MagicalKitties.Contracts.Requests.GlobalSetting;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MagicalKitties.API.Tests.Unit;

public class GlobalSettingControllerTests
{
    private readonly IGlobalSettingsService _globalSettingsService = Substitute.For<IGlobalSettingsService>();

    public GlobalSettingController _sut;

    public GlobalSettingControllerTests()
    {
        _sut = new GlobalSettingController(_globalSettingsService);
    }

    [Fact]
    public async Task Create_ReturnsValidatorError_WhenValidationFails()
    {
        // Arrange
        GlobalSettingCreateRequest request = new()
                                             {
                                                 Name = "Test",
                                                 Value = "false"
                                             };

        _globalSettingsService.CreateSettingAsync(Arg.Any<GlobalSetting>()).Throws(new ValidationException("Validation Failed"));

        // Act
        Func<Task<IActionResult>> result = async () => await _sut.Create(request, CancellationToken.None);

        // Assert
        await result.Should().ThrowAsync<ValidationException>("Validation Failed");
    }

    [Fact]
    public async Task Create_ReturnsGlobalSetting_WhenValidationPasses()
    {
        // Arrange
        GlobalSettingCreateRequest request = new()
                                             {
                                                 Name = "Test",
                                                 Value = "false"
                                             };
        _globalSettingsService.CreateSettingAsync(Arg.Any<GlobalSetting>()).Returns(true);

        // Act
        CreatedAtActionResult result = (CreatedAtActionResult)await _sut.Create(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(201);
        result.Value.Should().BeEquivalentTo(request.ToGlobalSetting(), options => options.Excluding(x => x.Id));
    }
}