using Bogus;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Specialized;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using MagicalKitties.Application.Models.Accounts;
using MagicalKitties.Application.Validators.Accounts;
using ValidationException = FluentValidation.ValidationException;

namespace MagicalKitties.Application.Tests.Unit.Validators;

public class GetAllAccountsOptionsValidatorTests
{
    public GetAllAccountsOptionsValidator _sut;

    public GetAllAccountsOptionsValidatorTests()
    {
        _sut = new GetAllAccountsOptionsValidator();
    }

    [Fact]
    public async Task Validator_ShouldThrowAsync_WhenSortFieldIsInvalid()
    {
        // Arrange
        GetAllAccountsOptions options = new()
                                        {
                                            SortField = "bacon",
                                            Page = 1,
                                            PageSize = 10
                                        };

        // Act
        TestValidationResult<GetAllAccountsOptions>? result = await _sut.TestValidateAsync(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x=>x.SortField).WithErrorMessage("You can only sort by Username or Lastlogin");
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    public async Task Validator_ShouldThrowAsync_WhenPageValueIsInvalid(int page)
    {
        // Arrange
        GetAllAccountsOptions options = new()
                                        {
                                            SortField = "username",
                                            Page = page,
                                            PageSize = 10
                                        };

        // Act
        TestValidationResult<GetAllAccountsOptions>? result = await _sut.TestValidateAsync(options);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(0)]
    [InlineData(26)]
    [InlineData(int.MaxValue)]
    public async Task Validator_ShouldThrowAsync_WhenPageSizeValueIsInvalid(int pageSize)
    {
        // Arrange
        GetAllAccountsOptions options = new()
                                        {
                                            SortField = "username",
                                            Page = 1,
                                            PageSize = pageSize
                                        };

        // Act
        TestValidationResult<GetAllAccountsOptions>? result = await _sut.TestValidateAsync(options);

        result.ShouldHaveValidationErrorFor(x => x.PageSize).WithErrorMessage("You can get between 1 and 25 accounts per page");
    }

    [Theory]
    [InlineData("username", 1, 10)]
    [InlineData("last_login_utc", 1, 10)]
    [InlineData(null, 5, 10)]
    [InlineData(null, 25, 10)]
    [InlineData(null, 1, 1)]
    [InlineData(null, 1, 15)]
    [InlineData(null, 1, 25)]
    public async Task Validator_DoesNotThrowError_WhenValidationSucceeds(string? sortField, int page, int pageSize)
    {
        // Arrange
        Faker<GetAllAccountsOptions>? options = new Faker<GetAllAccountsOptions>()
                                                .RuleFor(x => x.AccountRole, f => AccountRole.standard)
                                                .RuleFor(x => x.AccountStatus, f => AccountStatus.active)
                                                .RuleFor(x => x.SortField, _ => sortField)
                                                .RuleFor(x => x.Page, f => f.Random.Int(1, 100))
                                                .RuleFor(x => x.PageSize, f => f.Random.Int(1, 25));

        // Act
        TestValidationResult<GetAllAccountsOptions>? result = await _sut.TestValidateAsync(options);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}