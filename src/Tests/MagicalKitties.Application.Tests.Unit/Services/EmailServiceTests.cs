using FluentAssertions;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Repositories;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using NSubstitute;
using NSubstitute.Core;
using Testing.Common;

namespace MagicalKitties.Application.Tests.Unit.Services;

public class EmailServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IEmailRepository _emailRepository = Substitute.For<IEmailRepository>();

    public EmailServiceTests()
    {
        _sut = new EmailService(_emailRepository, _dateTimeProvider);
    }

    public EmailService _sut { get; set; }

    [Fact]
    public async Task QueueEmail_ShouldQueueEmail_WhenMethodIsCalled()
    {
        // Arrange
        DateTime now = DateTime.UtcNow;

        EmailData emailData = Fakes.GenerateEmailData(now);

        // Act
        await _sut.QueueEmailAsync(emailData);

        // Assert
        ICall? repositoryCall = _emailRepository.ReceivedCalls().FirstOrDefault();
        repositoryCall.Should().NotBeNull();

        EmailData? sentData = (EmailData?)repositoryCall.GetArguments().FirstOrDefault();
        sentData.Should().NotBeNull();
        sentData.Should().BeEquivalentTo(emailData, options => options.Excluding(x => x.Id).Excluding(x => x.ResponseLog));

        sentData.ResponseLog.Should().Contain("Email Queued;");
    }

    [Fact]
    public async Task GetForProcessingAsync_ShouldReturnEmptyList_WhenNothingIsAvailableForProcessing()
    {
        // Arrange
        _emailRepository.GetForProcessingAsync(Arg.Any<int>()).Returns([]);

        // Act
        List<EmailData> result = await _sut.GetForProcessingAsync(int.MaxValue);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetForProcessingAsync_ShouldReturnEmails_WhenItemsAreAvailable()
    {
        // Arrange
        List<EmailData> returnedItems = [Fakes.GenerateEmailData()];

        _emailRepository.GetForProcessingAsync(5).Returns(returnedItems);

        // Act
        List<EmailData> result = await _sut.GetForProcessingAsync(5);

        // Assert
        result.Should().BeEquivalentTo(returnedItems);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenUpdateFails()
    {
        // Arrange
        EmailData emailData = Fakes.GenerateEmailData();

        _emailRepository.UpdateAsync(Arg.Any<EmailData>()).Returns(false);

        // Act
        bool result = await _sut.UpdateAsync(emailData);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenUpdateSucceeds()
    {
        // Arrange
        EmailData emailData = Fakes.GenerateEmailData();

        _emailRepository.UpdateAsync(Arg.Any<EmailData>()).Returns(true);

        // Act
        bool result = await _sut.UpdateAsync(emailData);

        // Assert
        result.Should().BeTrue();
    }
}