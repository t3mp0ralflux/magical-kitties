namespace MagicalKitties.Contracts.Requests.Flaws;

public class GetAllFlawsRequest : PagedRequest
{
    public string? SortBy { get; init; }
}