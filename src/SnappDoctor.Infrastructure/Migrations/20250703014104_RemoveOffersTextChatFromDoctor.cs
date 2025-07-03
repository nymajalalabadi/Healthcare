using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOffersTextChatFromDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffersTextChat",
                table: "Doctors");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 41, 3, 878, DateTimeKind.Utc).AddTicks(5614));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 41, 3, 878, DateTimeKind.Utc).AddTicks(5622));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 41, 3, 878, DateTimeKind.Utc).AddTicks(5624));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OffersTextChat",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "OffersTextChat" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7178), true });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "OffersTextChat" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7185), true });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "OffersTextChat" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7187), true });
        }
    }
}
