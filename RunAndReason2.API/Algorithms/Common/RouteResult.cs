namespace RunAndReason2.API.Algorithms.Common;

/// <summary>
/// Result of a route optimization algorithm run (Nearest Neighbor, GA, ACO).
/// </summary>
public class RouteResult
{
    public List<int> LocationOrder { get; set; } = new();
    public double TotalDistance { get; set; }
    public long ExecutionTimeMs { get; set; }
    public List<double> ConvergenceHistory { get; set; } = new(); // empty for non-iterative algorithms like Nearest Neighbor
}