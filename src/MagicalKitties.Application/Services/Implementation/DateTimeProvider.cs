﻿namespace MagicalKitties.Application.Services.Implementation;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}