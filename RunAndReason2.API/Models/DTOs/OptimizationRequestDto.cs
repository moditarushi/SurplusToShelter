using System.ComponentModel.DataAnnotations;

namespace RunAndReason2.API.Models.DTOs;

public class OptimizationRequestDto
{
    [Required]
    public string Algorithm { get; set; } = string.Empty;

    [Required]
    public int DepotId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one customer location is required.")]
    public List<int> CustomerLocationIds { get; set; } = new();

    public AlgorithmConfigurationDto? Configuration { get; set; }
}