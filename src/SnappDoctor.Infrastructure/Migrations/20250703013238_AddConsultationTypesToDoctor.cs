using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnappDoctor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationTypesToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OffersInPersonConsultation",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OffersTextChat",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OffersVideoCall",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OffersVoiceCall",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "OffersInPersonConsultation", "OffersTextChat", "OffersVideoCall", "OffersVoiceCall" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7178), false, true, true, true });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "OffersInPersonConsultation", "OffersTextChat", "OffersVideoCall", "OffersVoiceCall" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7185), false, true, true, true });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "OffersInPersonConsultation", "OffersTextChat", "OffersVideoCall", "OffersVoiceCall" },
                values: new object[] { new DateTime(2025, 7, 3, 1, 32, 37, 986, DateTimeKind.Utc).AddTicks(7187), false, true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffersInPersonConsultation",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "OffersTextChat",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "OffersVideoCall",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "OffersVoiceCall",
                table: "Doctors");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(751));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(757));

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 1, 21, 24, 343, DateTimeKind.Utc).AddTicks(760));
        }
    }
}
