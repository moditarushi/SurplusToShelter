using System.ComponentModel.DataAnnotations;

namespace RunAndReason2.API.Models.DTOs;

public class ShortestPathRequestDto
{
    [Required]
    public string Algorithm { get; set; } = string.Empty; // "dijkstra" or "astar"

    [Required]
    public int StartLocationId { get; set; }

    [Required]
    public int EndLocationId { get; set; }
}