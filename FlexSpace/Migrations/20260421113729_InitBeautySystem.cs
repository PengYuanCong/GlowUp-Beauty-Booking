using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlexSpace.Migrations
{
    /// <inheritdoc />
    public partial class InitBeautySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Venues_VenueId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_OpeningHours_Venues_VenueId",
                table: "OpeningHours");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "OpeningHours",
                newName: "BeautyServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OpeningHours_VenueId",
                table: "OpeningHours",
                newName: "IX_OpeningHours_BeautyServiceId");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "Bookings",
                newName: "BeautyServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_VenueId",
                table: "Bookings",
                newName: "IX_Bookings_BeautyServiceId");

            migrationBuilder.CreateTable(
                name: "BeautyServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProviderId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeautyServices", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_BeautyServices_BeautyServiceId",
                table: "Bookings",
                column: "BeautyServiceId",
                principalTable: "BeautyServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpeningHours_BeautyServices_BeautyServiceId",
                table: "OpeningHours",
                column: "BeautyServiceId",
                principalTable: "BeautyServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_BeautyServices_BeautyServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_OpeningHours_BeautyServices_BeautyServiceId",
                table: "OpeningHours");

            migrationBuilder.DropTable(
                name: "BeautyServices");

            migrationBuilder.RenameColumn(
                name: "BeautyServiceId",
                table: "OpeningHours",
                newName: "VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_OpeningHours_BeautyServiceId",
                table: "OpeningHours",
                newName: "IX_OpeningHours_VenueId");

            migrationBuilder.RenameColumn(
                name: "BeautyServiceId",
                table: "Bookings",
                newName: "VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_BeautyServiceId",
                table: "Bookings",
                newName: "IX_Bookings_VenueId");

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Venues_VenueId",
                table: "Bookings",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OpeningHours_Venues_VenueId",
                table: "OpeningHours",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
