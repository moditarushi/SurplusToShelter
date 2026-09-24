namespace RunAndReason2.API.Models.DTOs;

public class AlgorithmConfigurationDto
{
    public int PopulationSize { get; set; } = 100;
    public double MutationRate { get; set; } = 0.05;
    public double CrossoverRate { get; set; } = 0.8;
    public int Generations { get; set; } = 500;
    public int ElitismCount { get; set; } = 2;
}