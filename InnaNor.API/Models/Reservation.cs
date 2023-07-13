namespace InnaNor.API.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public required int SpaceId { get; set; }
    public required Space Space { get; set; }
    public required string Reserver { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required string Notes { get; set; }
}
