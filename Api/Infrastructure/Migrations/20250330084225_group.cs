using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class group : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chargers_ChargerGroups_ChargingStationId",
                table: "Chargers");

            migrationBuilder.DropIndex(
                name: "IX_Chargers_ChargingStationId",
                table: "Chargers");

            migrationBuilder.DropColumn(
                name: "ChargingStationId",
                table: "Chargers");

            migrationBuilder.CreateIndex(
                name: "IX_Chargers_ChargerGroupId",
                table: "Chargers",
                column: "ChargerGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargers_ChargerGroups_ChargerGroupId",
                table: "Chargers",
                column: "ChargerGroupId",
                principalTable: "ChargerGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chargers_ChargerGroups_ChargerGroupId",
                table: "Chargers");

            migrationBuilder.DropIndex(
                name: "IX_Chargers_ChargerGroupId",
                table: "Chargers");

            migrationBuilder.AddColumn<Guid>(
                name: "ChargingStationId",
                table: "Chargers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Chargers_ChargingStationId",
                table: "Chargers",
                column: "ChargingStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargers_ChargerGroups_ChargingStationId",
                table: "Chargers",
                column: "ChargingStationId",
                principalTable: "ChargerGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
