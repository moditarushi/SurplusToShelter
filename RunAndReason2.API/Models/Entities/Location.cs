namespace RunAndReason2.API.Models.Entities;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Demand { get; set; } // 0 for depots, >0 for customers
    public bool IsDepot { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}