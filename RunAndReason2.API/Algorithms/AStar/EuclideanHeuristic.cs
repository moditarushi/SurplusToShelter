using RunAndReason2.API.Algorithms.Common;

namespace RunAndReason2.API.Algorithms.AStar;

public class EuclideanHeuristic : IHeuristic
{
    public string Name => "euclidean";

    public double Calculate(double lat1, double lon1, double lat2, double lon2)
    {
        return DistanceCalculator.EuclideanDistance(lat1, lon1, lat2, lon2);
    }
}