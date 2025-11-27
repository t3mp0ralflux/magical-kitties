namespace MagicalKitties.Application.Models.Rules;

public sealed class ProblemRule
{
        public required Guid Id { get; set; }
        public required string RollValue { get; init; }
        public required string Source { get; init; }
        public required string CustomSource { get; set; }
}