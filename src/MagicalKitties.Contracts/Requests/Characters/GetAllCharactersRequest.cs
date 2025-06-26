namespace MagicalKitties.Contracts.Requests.Characters;

public class GetAllCharactersRequest : PagedRequest
{
    public string? SearchInput { get; init; }
    public string? SortBy { get; init; }
}