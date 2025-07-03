using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationFeeToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationFee",
                table: "Doctors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConsultationFee", "CreatedAt" },
                values: new object[] { 150000m, new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(751) });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConsultationFee", "CreatedAt" },
                values: new object[] { 150000m, new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(757) });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConsultationFee", "CreatedAt" },
                values: new object[] { 150000m, new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(760) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationFee",
                table: "Doctors");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 2, 44, 21, DateTimeKind.Utc).AddTicks(7280));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 2, 44, 21, DateTimeKind.Utc).AddTicks(7288));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 2, 44, 21, DateTimeKind.Utc).AddTicks(7291));
        }
    }
}
