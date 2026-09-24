namespace RunAndReason2.API.Algorithms.Common;

public class PathResult
{
    public List<int> NodePath { get; set; } = new();
    public List<int> ExploredOrder { get; set; } = new();
    public double TotalDistance { get; set; }
    public long ExecutionTimeMs { get; set; }
    public bool PathFound { get; set; }
}