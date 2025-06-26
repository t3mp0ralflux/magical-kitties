using FluentValidation.TestHelper;
using MagicalKitties.Application.Models.Characters;
using MagicalKitties.Application.Validators.Characters;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class GetAllCharactersOptionsValidatorTests
{
    public GetAllCharactersOptionsValidator _sut;

    public GetAllCharactersOptionsValidatorTests()
    {
        _sut = new GetAllCharactersOptionsValidator();
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenSortFieldIsInvalid()
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              SortField = "bacon",
                                              Page = 1,
                                              PageSize = 10
                                          };
        // Act
        TestValidationResult<GetAllCharactersOptions>? result = await _sut.TestValidateAsync(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortField).WithErrorMessage("You can only sort by name or level");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    public async Task Validator_ShouldThrowAsync_WhenPageIsInvalid(int pageNumber)
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              SortField = "name",
                                              Page = pageNumber,
                                              PageSize = 10
                                          };
        // Act
        TestValidationResult<GetAllCharactersOptions>? result = await _sut.TestValidateAsync(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }
    
    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(26)]
    [InlineData(int.MaxValue)]
    public async Task Validator_ShouldThrowAsync_WhenPageSizeIsInvalid(int pageSize)
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              SortField = "name",
                                              Page = 1,
                                              PageSize = pageSize
                                          };
        // Act
        TestValidationResult<GetAllCharactersOptions>? result = await _sut.TestValidateAsync(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize).WithErrorMessage("You can get between 1 and 25 characters per page");
    }
    
    [Theory]
    [InlineData("name", 1, 10)]
    [InlineData("level", 1, 10)]
    [InlineData(null, 5, 10)]
    [InlineData(null, 25, 10)]
    [InlineData(null, 1, 1)]
    [InlineData(null, 1, 15)]
    [InlineData(null, 1, 25)]
    public async Task Validator_ShouldNotThrowAsync_WhenDataIsCorrect(string? sortField, int page, int pageSize)
    {
        // Arrange
        GetAllCharactersOptions options = new()
                                          {
                                              AccountId = Guid.NewGuid(),
                                              SortField = sortField,
                                              Page = page,
                                              PageSize = pageSize
                                          };
        // Act
        TestValidationResult<GetAllCharactersOptions>? result = await _sut.TestValidateAsync(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}