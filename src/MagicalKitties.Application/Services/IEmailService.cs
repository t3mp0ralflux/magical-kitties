using MagicalKitties.Application.Models.System;

namespace MagicalKitties.Application.Services;

public interface IEmailService
{
    Task QueueEmailAsync(EmailData emailData, CancellationToken token = default);
    Task<List<EmailData>> GetForProcessingAsync(int batchSize, CancellationToken token = default);
    Task<bool> UpdateAsync(EmailData emailData, CancellationToken token = default);
}