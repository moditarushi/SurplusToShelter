namespace RunAndReason2.API.Algorithms.AStar;

public class ManhattanHeuristic : IHeuristic
{
    public string Name => "manhattan";

    public double Calculate(double lat1, double lon1, double lat2, double lon2)
    {
        return Math.Abs(lat2 - lat1) + Math.Abs(lon2 - lon1);
    }
}