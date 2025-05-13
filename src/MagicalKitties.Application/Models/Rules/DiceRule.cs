namespace MagicalKitties.Application.Models.Rules;

public static class DiceRule
{
    public static readonly List<DiceDifficulty> DiceDifficulties =
    [
        new()
        {
            Difficulty = 3,
            DifficultyText = "(Easy)",
            CuteAction = "Get a human to feed you.",
            CunningAction = "Open a human door.",
            FierceAction = "Catch a mouse."
        },
        new()
        {
            Difficulty = 4,
            DifficultyText = "(Typical)",
            CuteAction = "Cheer up a sad kitty friend.",
            CunningAction = "Find a hidden compartment.",
            FierceAction = "Run through a door before it closes."
        },

        new()
        {
            Difficulty = 5,
            DifficultyText = "(Hard)",
            CuteAction = "Befriend a worker-bot.",
            CunningAction = "Read a textbook.",
            FierceAction = "Fight off a guard dog."
        },

        new()
        {
            Difficulty = 6,
            DifficultyText = "(Extreme)",
            CuteAction = "Herd cats.",
            CunningAction = "Solve the Riddle of the Sphinx.",
            FierceAction = "Fight off a dragon."
        }
    ];

    public static readonly List<DiceSuccess> DiceSuccesses =
    [
        new()
        {
            Successes = 0,
            Result = "Failure",
            Enhancements = "You don't do what you wanted, and may have a complication."
        },
        new()
        {
            Successes = 1,
            Result = "Success, But...",
            Enhancements = "You do it, and deal one Owie if trying to, but there's a complication."
        },

        new()
        {
            Successes = 2,
            Result = "Success",
            Enhancements = "You do it just like you hoped, and deal one Owie if trying to."
        },

        new()
        {
            Successes = 3,
            Result = "Success, and...",
            Enhancements = "You do it, and deal one Owie if trying to, plus get a bonus."
        },
        new()
        {
            Successes = 4,
            Result = "Super Success!",
            Enhancements = "You do it, and deal one Owie if trying to, plus get a super bonus."
        }
    ];

    public class DiceDifficulty
    {
        public required int Difficulty { get; set; }
        public required string DifficultyText { get; set; }
        public required string CuteAction { get; set; }
        public required string CunningAction { get; set; }
        public required string FierceAction { get; set; }
    }

    public class DiceSuccess
    {
        public required int Successes { get; set; }
        public required string Result { get; set; }
        public required string Enhancements { get; set; }
    }
}