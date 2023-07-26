namespace InnaNor.API.DTOs;

public class ReservationCreationDto
{
    public int SpaceId { get; set; }
    public required string Reserver { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public required string Notes { get; set; }
}
