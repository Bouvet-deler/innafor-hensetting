using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnaNor.API.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Norwegian_Column_Name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Beskrivelse",
                table: "Spaces",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Spaces",
                newName: "Beskrivelse");
        }
    }
}
