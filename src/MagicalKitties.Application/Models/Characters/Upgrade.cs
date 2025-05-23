﻿using MagicalKitties.Application.Models.Characters.Updates;

namespace MagicalKitties.Application.Models.Characters;

public class Upgrade
{
    public required Guid Id { get; init; }
    public required int Block { get; init; }
    public required int Level { get; init; }
    public AttributeOption Option { get; set; }
    public object? Choice { get; set; }
}