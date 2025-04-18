namespace MagicalKitties.Contracts.Requests.Talents;

public class GetAllTalentsRequest : PagedRequest
{
    public string? SortBy { get; init; }
}