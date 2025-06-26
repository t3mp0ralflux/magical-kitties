namespace MagicalKitties.Application.Models.Characters;

public class GetAllCharactersOptions : SharedGetAllOptions
{
    public required Guid AccountId { get; init; }
    public string? SearchInput { get; init; }
}