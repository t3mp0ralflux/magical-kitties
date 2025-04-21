namespace MagicalKitties.Contracts.Requests.Endowments.MagicalPowers;

public class GetAllMagicalPowersRequest : PagedRequest
{
    public string? SortBy { get; init; }
}