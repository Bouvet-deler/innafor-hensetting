namespace InnaNor.API.DTOs;

public class ReservationOverviewDto
{
    public Guid Id { get; set; }
    public int SpaceId { get; set; }
    public required string SpaceName { get; set; }
    public required string Reserver { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
