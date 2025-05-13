namespace MagicalKitties.Application.Models.Rules;

public static class ProblemRule
{
    public static readonly List<Emotion> EmotionSources =
    [
        // TODO: will need to add roll range array to ensure that the FE roller can parse these into something usable.
        new()
        {
            RollValue = "11-13",
            EmotionSource = "Sad"
        },
        new()
        {
            RollValue = "14-16",
            EmotionSource = "Angry"
        },
        new()
        {
            RollValue = "21-23",
            EmotionSource = "Scared"
        },
        new()
        {
            RollValue = "24-26",
            EmotionSource = "Disappointed"
        },
        new()
        {
            RollValue = "31-33",
            EmotionSource = "Guilty"
        },
        new()
        {
            RollValue = "34-36",
            EmotionSource = "Anxious"
        },
        new()
        {
            RollValue = "41-43",
            EmotionSource = "Apathetic"
        },
        new()
        {
            RollValue = "44-46",
            EmotionSource = "Ashamed"
        },
        new()
        {
            RollValue = "51-53",
            EmotionSource = "Jealous"
        },
        new()
        {
            RollValue = "54-56",
            EmotionSource = "Confused"
        },
        new()
        {
            RollValue = "61-63",
            EmotionSource = "Frustrated"
        },
        new()
        {
            RollValue = "64-66",
            EmotionSource = "Depressed"
        },
        new()
        {
            RollValue = "99",
            EmotionSource = "Custom"
        }
    ];

    public static readonly List<Problem> ProblemSources =
    [
        new()
        {
            RollValue = 1,
            ProblemSource = "Money"
        },
        new()
        {
            RollValue = 2,
            ProblemSource = "Health"
        },
        new()
        {
            RollValue = 3,
            ProblemSource = "Family"
        },
        new()
        {
            RollValue = 4,
            ProblemSource = "Friend/Enemy"
        },
        new()
        {
            RollValue = 5,
            ProblemSource = "Work/School"
        },
        new()
        {
            RollValue = 6,
            ProblemSource = "Neighborhood"
        },
        new()
        {
            RollValue = 99,
            ProblemSource = "Custom"
        }
    ];

    public class Problem
    {
        public required int RollValue { get; set; }
        public required string ProblemSource { get; set; }
    }

    public class Emotion
    {
        public required string RollValue { get; set; }
        public required string EmotionSource { get; set; }
    }
}