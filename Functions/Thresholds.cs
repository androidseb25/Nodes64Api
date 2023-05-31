using System;
namespace Nodes64Api.Functions
{
	public class ThresholdsFunction
    {
        public string Color(Thresholds th)
        {
            switch (th)
            {
                case Thresholds.VeryBad:
                    return "#dc3545";
                case Thresholds.Bad:
                    return "#b58802";
                case Thresholds.VeryMedium:
                    return "#6c757d";
                case Thresholds.Medium:
                    return "#007bff";
                case Thresholds.Good:
                    return "#17a2b8";
                case Thresholds.VeryGood:
                    return "#28a745";
                default:
                    return "#dc3545";
            }
        }

        public string Text(Thresholds th)
        {
            switch (th)
            {
                case Thresholds.VeryBad:
                    return "sehr schlecht";
                case Thresholds.Bad:
                    return "schlecht";
                case Thresholds.VeryMedium:
                    return "mittelschlecht";
                case Thresholds.Medium:
                    return "mittel";
                case Thresholds.Good:
                    return "gut";
                case Thresholds.VeryGood:
                    return "sehr gut";
                default:
                    return "sehr schlecht";
            }
        }

        public Thresholds GetThreadhold(decimal avg)
        {
            switch (avg)
            {
                case <= 0: return Thresholds.VeryBad;
                case > 0 and <= 18: return Thresholds.VeryGood;
                case > 18 and <= 22: return Thresholds.Good;
                case > 22 and <= 26: return Thresholds.Medium;
                case > 26 and <= 35: return Thresholds.VeryMedium;
                case > 35 and <= 50: return Thresholds.Bad;
                default: return Thresholds.VeryBad;
            }
        }

        public Thresholds GetThreadholdPackageLoss(decimal pkgLoss)
        {
            switch (pkgLoss)
            {
                case <= 0: return Thresholds.VeryGood;
                case > 0: return Thresholds.VeryBad;
            }
        }
    }
	public enum Thresholds
	{
		VeryBad,
		Bad,
        VeryMedium,
        Medium,
        Good,
		VeryGood
	}
}

