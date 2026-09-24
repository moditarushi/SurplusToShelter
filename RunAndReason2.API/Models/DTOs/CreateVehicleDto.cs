using System.ComponentModel.DataAnnotations;

namespace RunAndReason2.API.Models.DTOs;

public class CreateVehicleDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    [Required]
    public int StartingDepotId { get; set; }
}