﻿namespace MagicalKitties.Application.Models.Characters;

public class GetAllCharactersOptions : SharedGetAllOptions
{
    public required Guid AccountId { get; init; }
    public string? Name { get; init; }
    public int? Level { get; init; }
    public string? Class { get; init; }
    public string? Species { get; init; }
}