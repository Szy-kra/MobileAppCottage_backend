using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileAppCottage.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "CottageReservations",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "CottageReservations",
                newName: "From");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "To",
                table: "CottageReservations",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "CottageReservations",
                newName: "EndDate");
        }
    }
}
