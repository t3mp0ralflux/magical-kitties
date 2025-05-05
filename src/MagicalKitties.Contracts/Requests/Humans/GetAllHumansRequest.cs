namespace MagicalKitties.Contracts.Requests.Humans;

public class GetAllHumansRequest : PagedRequest
{
    public required Guid CharacterId { get; init; }
    public string? Name { get; init; }
    public string? SortBy { get; init; }
}