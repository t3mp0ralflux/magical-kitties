﻿using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using MagicalKitties.Application.Models.System;
using MagicalKitties.Application.Services;
using MagicalKitties.Application.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MagicalKitties.Application.HostedServices;

public class EmailProcessingService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEmailService _emailService;
    private readonly IGlobalSettingsService _globalSettingsService;
    private readonly ILogger<EmailProcessingService> _logger;

    private PeriodicTimer _timer = new(TimeSpan.FromSeconds(9999)); // initial value to prevent running for a while. Overridden in StartAsync.

    public EmailProcessingService(ILogger<EmailProcessingService> logger, IEmailService emailService, IDateTimeProvider dateTimeProvider, IGlobalSettingsService globalSettingsService, IConfiguration configuration)
    {
        _logger = logger;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
        _globalSettingsService = globalSettingsService;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("EmailProcessing Service started");

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        State state = new();

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessEmailsAsync(state, cancellationToken);
            await _timer.WaitForNextTickAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        _logger.LogInformation("EmailProcessing Service ended");
        return Task.CompletedTask;
    }

    private async Task ProcessEmailsAsync(State state, CancellationToken token)
    {
        if (state.IsRunning)
        {
            return;
        }

        try
        {
            int maxEmailsToSend = await _globalSettingsService.GetSettingAsync(WellKnownGlobalSettings.EMAIL_SEND_BATCH_LIMIT, 100, token);

            List<EmailData> emailsToProcess = await _emailService.GetForProcessingAsync(maxEmailsToSend, token);

            if (emailsToProcess.Count == 0)
            {
                state.IsRunning = false;
                return;
            }

            int maxAttempts = await _globalSettingsService.GetSettingAsync(WellKnownGlobalSettings.EMAIL_SEND_ATTEMPTS_MAX, 5, token);

            foreach (EmailData emailData in emailsToProcess)
            {
                emailData.SendAttempts++;

                if (emailData.SendAttempts > maxAttempts)
                {
                    emailData.ShouldSend = false;
                    emailData.ResponseLog += $"{_dateTimeProvider}: Max email attempts reached";

                    await _emailService.UpdateAsync(emailData, token);
                    continue;
                }

                emailData.ShouldSend = false; // hit early to avoid spamming on DB write errors.

                await _emailService.UpdateAsync(emailData, token);

                (bool success, string message) = await SendEmailAsync(emailData, token);

                if (success)
                {
                    emailData.ResponseLog += $"{_dateTimeProvider}: Email Sent;";
                    emailData.ShouldSend = false;
                    emailData.SentUtc = _dateTimeProvider.GetUtcNow();

                    await _emailService.UpdateAsync(emailData, token);
                    continue;
                }

                emailData.ShouldSend = true;
                emailData.ResponseLog += $"{_dateTimeProvider.GetUtcNow()}: Email failed to send. Attempt {emailData.SendAttempts} out of {maxAttempts}. Error {message};";
                await _emailService.UpdateAsync(emailData, token);
            }
        }
        catch (NpgsqlException dbException)
        {
            if (dbException.InnerException is SocketException)
            {
                _logger.LogCritical("Could not reach database. Error: {Message}", dbException.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private async Task<(bool success, string message)> SendEmailAsync(EmailData emailData, CancellationToken token)
    {
        MailAddress from = new(emailData.SenderEmail, "MagicalKitties Account Services");
        MailAddress to = new(emailData.RecipientEmail);

        MailMessage message = new(from, to);
        message.Subject = "Account Activation";
        message.IsBodyHtml = true;
        message.Body = emailData.Body;

        SmtpClient client = new("smtp.gmail.com", 587);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(_configuration["Google:EmailAddress"], _configuration["Google:EmailCode"]);

        try
        {
            await client.SendMailAsync(message, token);
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogWarning(smtpEx.Message, smtpEx);
            return (false, smtpEx.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return (false, e.Message);
        }

        return (true, "Email sent successfully");
    }

    private class State
    {
        public bool IsRunning { get; set; }
        public int Count { get; set; }
        public CancellationToken token { get; set; }
    }
}