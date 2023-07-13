using Microsoft.EntityFrameworkCore;
// Disable nullable warnings for DbSet as Ef Core initializes them
#pragma warning disable CS8618

namespace InnaNor.API.Models;

public class InnaNorContext : DbContext
{
    public InnaNorContext(DbContextOptions<InnaNorContext> options)
        : base(options)
    {
    }

    public DbSet<Space> Spaces { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
}
