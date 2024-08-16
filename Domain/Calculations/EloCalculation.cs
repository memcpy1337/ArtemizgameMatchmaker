using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Calculations;

public static class EloCalculation
{
    private const int Kfactor = 30;
    public static double CalculateProbability(double ratingA, double ratingB)
    {
        return 1.0 / (1.0 + Math.Pow(10, (ratingB - ratingA) / 400.0));
    }

    public static double UpdateRating(double currentRating, double actualScore, double expectedScore)
    {
        return currentRating + Kfactor * (actualScore - expectedScore);
    }
}
