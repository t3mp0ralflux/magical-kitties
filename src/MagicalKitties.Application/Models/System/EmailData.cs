namespace MagicalKitties.Application.Models.System;

public class EmailData
{
    public required Guid Id { get; init; }
    public required Guid SenderAccountId { get; init; }
    public required Guid ReceiverAccountId { get; init; }
    public required bool ShouldSend { get; set; } = true;
    public required int SendAttempts { get; set; }
    public DateTime? SentUtc { get; set; }
    public required DateTime SendAfterUtc { get; init; }
    public required string SenderEmail { get; init; }
    public required string RecipientEmail { get; init; }
    public required string Body { get; init; }
    public required string ResponseLog { get; set; }
}