namespace RunAndReason2.API.Models.DTOs;

public class OptimizationResultDto
{
    public string Algorithm { get; set; } = string.Empty;
    public List<int> RouteOrder { get; set; } = new();
    public double TotalDistanceKm { get; set; }
    public long ExecutionTimeMs { get; set; }
    public List<double> ConvergenceHistory { get; set; } = new();
}