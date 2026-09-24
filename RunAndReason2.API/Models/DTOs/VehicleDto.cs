namespace RunAndReason2.API.Models.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int StartingDepotId { get; set; }
    public bool IsActive { get; set; }
}