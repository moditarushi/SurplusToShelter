namespace RunAndReason2.API.Models.DTOs;

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Demand { get; set; }
    public bool IsDepot { get; set; }
}