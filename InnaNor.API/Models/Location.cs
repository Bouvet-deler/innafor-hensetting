using System.ComponentModel.DataAnnotations;

namespace InnaNor.API.Models;

public class Location
{
    [StringLength(10)]
    public required string Id { get; set; }
    
    public required string Status { get; set; }
    public string? TrackNumber { get; set; }

    public string? TrackResponsible { get; set; }

    public string? Area { get; set; }

    public string? Track { get; set; }
}
