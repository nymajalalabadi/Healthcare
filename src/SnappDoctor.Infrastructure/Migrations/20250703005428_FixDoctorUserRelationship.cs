using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDoctorUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId1",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId1",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Doctors");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 0, 54, 28, 296, DateTimeKind.Utc).AddTicks(3208));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 0, 54, 28, 296, DateTimeKind.Utc).AddTicks(3216));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 0, 54, 28, 296, DateTimeKind.Utc).AddTicks(3218));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UserId1" },
                values: new object[] { new DateTime(2025, 7, 3, 0, 53, 32, 941, DateTimeKind.Utc).AddTicks(2379), null });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UserId1" },
                values: new object[] { new DateTime(2025, 7, 3, 0, 53, 32, 941, DateTimeKind.Utc).AddTicks(2386), null });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UserId1" },
                values: new object[] { new DateTime(2025, 7, 3, 0, 53, 32, 941, DateTimeKind.Utc).AddTicks(2388), null });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId1",
                table: "Doctors",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId1",
                table: "Doctors",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
