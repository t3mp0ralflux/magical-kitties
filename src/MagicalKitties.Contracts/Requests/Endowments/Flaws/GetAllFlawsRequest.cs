namespace MagicalKitties.Contracts.Requests.Endowments.Flaws;

public class GetAllFlawsRequest : PagedRequest
{
    public string? SortBy { get; init; }
}