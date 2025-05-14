namespace MagicalKitties.Application.Models.GlobalSettings;

public class GetAllGlobalSettingsOptions
{
    public string? Name { get; init; }
    public string? SortField { get; init; }
    public SortOrder? SortOrder { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}