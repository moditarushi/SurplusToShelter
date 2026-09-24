namespace RunAndReason2.API.Algorithms.AStar;

public interface IHeuristic
{
    string Name { get; }
    double Calculate(double lat1, double lon1, double lat2, double lon2);
}