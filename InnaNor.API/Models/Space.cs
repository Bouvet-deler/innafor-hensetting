using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnaNor.API.Models;

public class Space
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [StringLength(13)]
    public required string GlobalId { get; set; }
    public required string LocationId { get; set; }
    public required Location Location { get; set; }

    public string? Beskrivelse { get; set; }

    public required string Name { get; set; }

    public decimal From { get; set; }

    public decimal To { get; set; }

    public required string TrackType { get; set; }

    public required string TrackId { get; set; }

    public required string Status { get; set; }

    public required string LastChangedBy { get; set; }

    public DateTime LastChanged { get; set; }

    [StringLength(13)]
    public required string BelongsTo { get; set; }

    public int TrackPriority { get; set; }

    public DateOnly? ActiveFrom { get; set; }

    public required string Owner { get; set; }

    public required string TrackUsageType { get; set; }

    // Meters
    public decimal? TrainPlacementLength { get; set; }

    // Meters
    public decimal? Length { get; set; }

    public string? FromSignal { get; set; }

    public string? ToSignal { get; set; }

    // public string? KlAnlegg { get; set; }

    // public string? NummerPåSkillebryter { get; set; }

    public string? ServiceRamp { get; set; }

    public string? DeIcing { get; set; }

    public string? WaterFilling { get; set; }

    public string? TrainWashing { get; set; }

    public string? DieselRefueling { get; set; }

    public string? Notes { get; set; }

    // public string? HelsveistLasket { get; set; }

    // public string? ServicekioskVvsStrøm { get; set; }

    public string? ServiceHouseForCleaningSuppliers { get; set; }

    public string? GraffitiRemoval { get; set; }

    public string? AccessibleByCarKioskResupplyAndService { get; set; }

    public string? SewageEmptyingByCar { get; set; }

    public string? SewageEmptyingStationary { get; set; }


    // UTM 32
    public string? StartNorth { get; set; }

    // UTM 32
    public string? StartEast { get; set; }

    // UTM 32
    public string? EndNorth { get; set; }

    // UTM 32
    public string? EndEast { get; set; }
}
