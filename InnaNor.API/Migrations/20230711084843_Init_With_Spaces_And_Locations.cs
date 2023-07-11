using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnaNor.API.Migrations
{
    /// <inheritdoc />
    public partial class Init_With_Spaces_And_Locations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TrackNumber = table.Column<string>(type: "TEXT", nullable: true),
                    TrackResponsible = table.Column<string>(type: "TEXT", nullable: true),
                    Area = table.Column<string>(type: "TEXT", nullable: true),
                    Track = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    GlobalId = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    LocationId = table.Column<string>(type: "TEXT", nullable: false),
                    Beskrivelse = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    From = table.Column<decimal>(type: "TEXT", nullable: false),
                    To = table.Column<decimal>(type: "TEXT", nullable: false),
                    TrackType = table.Column<string>(type: "TEXT", nullable: false),
                    TrackId = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    LastChangedBy = table.Column<string>(type: "TEXT", nullable: false),
                    LastChanged = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BelongsTo = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    TrackPriority = table.Column<int>(type: "INTEGER", nullable: false),
                    ActiveFrom = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Owner = table.Column<string>(type: "TEXT", nullable: false),
                    TrackUsageType = table.Column<string>(type: "TEXT", nullable: false),
                    TrainPlacementLength = table.Column<decimal>(type: "TEXT", nullable: true),
                    Length = table.Column<decimal>(type: "TEXT", nullable: true),
                    FromSignal = table.Column<string>(type: "TEXT", nullable: true),
                    ToSignal = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceRamp = table.Column<string>(type: "TEXT", nullable: true),
                    DeIcing = table.Column<string>(type: "TEXT", nullable: true),
                    WaterFilling = table.Column<string>(type: "TEXT", nullable: true),
                    TrainWashing = table.Column<string>(type: "TEXT", nullable: true),
                    DieselRefueling = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    ServiceHouseForCleaningSuppliers = table.Column<string>(type: "TEXT", nullable: true),
                    GraffitiRemoval = table.Column<string>(type: "TEXT", nullable: true),
                    AccessibleByCarKioskResupplyAndService = table.Column<string>(type: "TEXT", nullable: true),
                    SewageEmptyingByCar = table.Column<string>(type: "TEXT", nullable: true),
                    SewageEmptyingStationary = table.Column<string>(type: "TEXT", nullable: true),
                    StartNorth = table.Column<string>(type: "TEXT", nullable: true),
                    StartEast = table.Column<string>(type: "TEXT", nullable: true),
                    EndNorth = table.Column<string>(type: "TEXT", nullable: true),
                    EndEast = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spaces_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_GlobalId",
                table: "Spaces",
                column: "GlobalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_LocationId",
                table: "Spaces",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spaces");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
