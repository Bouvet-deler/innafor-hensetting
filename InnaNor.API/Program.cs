using InnaNor.API.Models;
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
    db.Spaces.Select(s => new
    {
        s.Id,
        s.Name,
        s.Description,
    }));
app.MapGet("/spaces/{id:int}", (int id, InnaNorContext db) => db.Spaces.Find(id));
app.MapGet("/spaces/{id:int}/reservations", (int id, InnaNorContext db) => db.Reservations.Where(r => r.SpaceId == id));

app.MapGet("/locations", (InnaNorContext db) =>
    db.Locations.Select(l => new
    {
        l.Id,
        l.TrackNumber,
        l.Area,
        l.Status,
    }));
app.MapGet("/locations/{id:int}", (int id, InnaNorContext db) => db.Locations.Find(id));

app.MapGet("/reservations", (InnaNorContext db) =>
    db.Reservations.Select(r => new
    {
        r.Id,
        SpaceId = r.Space.Id,
        r.Space.Name,
        r.Reserver,
        r.StartTime,
        r.EndTime,
    }));
app.MapGet("/reservations/{id:int}", (int id, InnaNorContext db) => db.Reservations.Find(id));

app.Run();
