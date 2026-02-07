using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobileAppCottage.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHostToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CottageReservations_AspNetUsers_ReservedById",
                table: "CottageReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Cottages_AspNetUsers_OwnerId",
                table: "Cottages");

            migrationBuilder.AddColumn<bool>(
                name: "IsHost",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_CottageReservations_AspNetUsers_ReservedById",
                table: "CottageReservations",
                column: "ReservedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cottages_AspNetUsers_OwnerId",
                table: "Cottages",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CottageReservations_AspNetUsers_ReservedById",
                table: "CottageReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Cottages_AspNetUsers_OwnerId",
                table: "Cottages");

            migrationBuilder.DropColumn(
                name: "IsHost",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_CottageReservations_AspNetUsers_ReservedById",
                table: "CottageReservations",
                column: "ReservedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cottages_AspNetUsers_OwnerId",
                table: "Cottages",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
