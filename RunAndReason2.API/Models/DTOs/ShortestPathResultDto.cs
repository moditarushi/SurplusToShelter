namespace RunAndReason2.API.Models.DTOs;

public class ShortestPathResultDto
{
    public string Algorithm { get; set; } = string.Empty;
    public List<int> NodePath { get; set; } = new();
    public List<int> ExploredOrder { get; set; } = new();
    public double TotalDistanceKm { get; set; }
    public long ExecutionTimeMs { get; set; }
    public bool PathFound { get; set; }
}