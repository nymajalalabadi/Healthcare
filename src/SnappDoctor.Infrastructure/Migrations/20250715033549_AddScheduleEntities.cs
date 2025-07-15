using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 35, 48, 326, DateTimeKind.Utc).AddTicks(2966));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 35, 48, 326, DateTimeKind.Utc).AddTicks(2974));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 35, 48, 326, DateTimeKind.Utc).AddTicks(2976));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 22, 1, 618, DateTimeKind.Utc).AddTicks(3170));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 22, 1, 618, DateTimeKind.Utc).AddTicks(3178));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 15, 3, 22, 1, 618, DateTimeKind.Utc).AddTicks(3181));
        }
    }
}
