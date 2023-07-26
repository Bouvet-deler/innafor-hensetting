namespace InnaNor.API.DTOs;

public class LocationOverviewDto
{
    public required string Id { get; set; }
    public string? TrackNumber { get; set; }
    public string? Area { get; set; }
    public required string Status { get; set; }
}
