namespace MagicalKitties.Application.Models.Rules;

public static class ProblemRule
{
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

    public static readonly List<Problem> ProblemSources =
    [
        new Problem
        {
            RollValue = 1,
            ProblemSource = "Money"
        },
        new Problem
        {
            RollValue = 2,
            ProblemSource = "Health"
        },
        new Problem
        {
            RollValue = 3,
            ProblemSource = "Family"
        },
        new Problem
        {
            RollValue = 4,
            ProblemSource = "Friend/Enemy"
        },
        new Problem
        {
            RollValue = 5,
            ProblemSource = "Work/School"
        },
        new Problem
        {
            RollValue = 6,
            ProblemSource = "Neighborhood"
        },
        new Problem
        {
            RollValue = 99,
            ProblemSource = "Custom"
        }
    ];

    public static readonly List<Emotion> EmotionSources = 
    [
        // TODO: will need to add roll range array to ensure that the FE roller can parse these into something usable.
        new Emotion
        {
            RollValue = "11-13",
            EmotionSource = "Sad"
        },
        new Emotion
        {
            RollValue = "14-16",
            EmotionSource = "Angry"
        },
        new Emotion
        {
            RollValue = "21-23",
            EmotionSource = "Scared"
        },
        new Emotion
        {
            RollValue = "24-26",
            EmotionSource = "Disappointed"
        },
        new Emotion
        {
            RollValue = "31-33",
            EmotionSource = "Guilty"
        },
        new Emotion
        {
            RollValue = "34-36",
            EmotionSource = "Anxious"
        },
        new Emotion
        {
            RollValue = "41-43",
            EmotionSource = "Apathetic"
        },
        new Emotion
        {
            RollValue = "44-46",
            EmotionSource = "Ashamed"
        },
        new Emotion
        {
            RollValue = "51-53",
            EmotionSource = "Jealous"
        },
        new Emotion
        {
            RollValue = "54-56",
            EmotionSource = "Confused"
        },
        new Emotion
        {
            RollValue = "61-63",
            EmotionSource = "Frustrated"
        },
        new Emotion
        {
            RollValue = "64-66",
            EmotionSource = "Depressed"
        },
        new Emotion
        {
            RollValue = "99",
            EmotionSource = "Custom"
        }
    ];
}