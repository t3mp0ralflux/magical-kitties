﻿namespace MagicalKitties.Contracts.Responses.Characters;

public class GetAllCharacterResponse
{
    public required Guid Id { get; set; }
    public required Guid AccountId { get; set; }
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required int Level { get; set; }
    public required List<string> MagicalPowers { get; set; }
    public required string? HumanName { get; set; }
}