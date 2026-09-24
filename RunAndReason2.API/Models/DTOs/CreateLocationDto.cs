using System.ComponentModel.DataAnnotations;

namespace RunAndReason2.API.Models.DTOs;

public class CreateLocationDto
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [Range(0, int.MaxValue)]
    public int Demand { get; set; }

    public bool IsDepot { get; set; }
}