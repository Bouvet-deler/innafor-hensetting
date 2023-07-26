using InnaNor.API.DTOs;
using InnaNor.API.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string allowLocalhostCors = "AllowLocalhostCors";
builder.Services.AddCors(
        options =>
            options.AddPolicy(allowLocalhostCors, policyBuilder =>
                policyBuilder
                    // .AllowAnyOrigin()
                    .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            )
    )
    ;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InnaNorContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("InnaNorContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allowLocalhostCors);

app.UseAuthorization();

// app.MapControllers();

app.MapGet("/spaces", (InnaNorContext db) =>
    db.Spaces.Select(s => new SpaceOverviewDto
    {
        Id = s.Id,
        Name = s.Name,
        Description = s.Description,
    }));
app.MapGet("/spaces/{id:int}", (int id, InnaNorContext db) => db.Spaces.Find(id));
app.MapGet("/spaces/{id:int}/reservations", (int id, InnaNorContext db) => db.Reservations.Where(r => r.SpaceId == id));

app.MapGet("/locations", (InnaNorContext db) =>
    db.Locations.Select(l => new LocationOverviewDto
    {
        Id = l.Id,
        TrackNumber = l.TrackNumber,
        Area = l.Area,
        Status = l.Status,
    }));
app.MapGet("/locations/{id}", (string id, InnaNorContext db) => db.Locations.Find(id));

app.MapGet("/reservations", (InnaNorContext db) =>
    db.Reservations.Select(r => new ReservationOverviewDto
    {
        Id = r.Id,
        SpaceId = r.Space.Id,
        SpaceName = r.Space.Name,
        Reserver = r.Reserver,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
    }));
app.MapPost("/reservations",
    Results<Created<Reservation>, ValidationProblem> (ReservationCreationDto reservation, InnaNorContext db) =>
    {
        var space = db.Spaces.Find(reservation.SpaceId);
        if (space == null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                { { "SpaceId", new[] { $"No spaces found with the given SpaceId {reservation.SpaceId}" } } });
        var createdReservation = db.Reservations.Add(new Reservation
        {
            SpaceId = reservation.SpaceId,
            Space = space,
            Reserver = reservation.Reserver,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            Notes = reservation.Notes,
        }).Entity;
        db.SaveChanges();
        return TypedResults.Created($"/reservations/${createdReservation.Id}", createdReservation);
    });
app.MapGet("/reservations/{id:guid}",
    Results<Ok<Reservation>, NotFound> (Guid id, InnaNorContext db) =>
    {
        var reservation = db.Reservations
            .Include(r => r.Space)
            .First(r => r.Id == id);
        return TypedResults.Ok(reservation);
    });

app.Run();
