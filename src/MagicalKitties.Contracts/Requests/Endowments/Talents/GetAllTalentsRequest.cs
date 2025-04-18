namespace MagicalKitties.Contracts.Requests.Endowments.Talents;

public class GetAllTalentsRequest : PagedRequest
{
    public string? SortBy { get; init; }
}