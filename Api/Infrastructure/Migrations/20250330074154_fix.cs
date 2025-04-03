using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 96, DateTimeKind.Utc).AddTicks(4325),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 528, DateTimeKind.Utc).AddTicks(9697));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 96, DateTimeKind.Utc).AddTicks(2711),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 528, DateTimeKind.Utc).AddTicks(8806));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 42, DateTimeKind.Utc).AddTicks(7987),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 478, DateTimeKind.Utc).AddTicks(2986));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 41, DateTimeKind.Utc).AddTicks(817),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 476, DateTimeKind.Utc).AddTicks(4148));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 528, DateTimeKind.Utc).AddTicks(9697),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 96, DateTimeKind.Utc).AddTicks(4325));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 528, DateTimeKind.Utc).AddTicks(8806),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 96, DateTimeKind.Utc).AddTicks(2711));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 478, DateTimeKind.Utc).AddTicks(2986),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 42, DateTimeKind.Utc).AddTicks(7987));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 13, 10, 14, 40, 476, DateTimeKind.Utc).AddTicks(4148),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 3, 30, 7, 41, 53, 41, DateTimeKind.Utc).AddTicks(817));
        }
    }
}
