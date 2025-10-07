using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialdatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 55, 48, 82, DateTimeKind.Utc).AddTicks(2123));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 55, 48, 82, DateTimeKind.Utc).AddTicks(2130));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 55, 48, 82, DateTimeKind.Utc).AddTicks(2133));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 53, 54, 523, DateTimeKind.Utc).AddTicks(3464));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 53, 54, 523, DateTimeKind.Utc).AddTicks(3473));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 10, 1, 0, 53, 54, 523, DateTimeKind.Utc).AddTicks(3476));
        }
    }
}
