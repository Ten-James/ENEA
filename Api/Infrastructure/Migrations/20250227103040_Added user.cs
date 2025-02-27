using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addeduser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 571, DateTimeKind.Local).AddTicks(3430),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 23, 13, 42, 3, 999, DateTimeKind.Local).AddTicks(8557));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 567, DateTimeKind.Local).AddTicks(4464),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 23, 13, 42, 3, 995, DateTimeKind.Local).AddTicks(8664));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 622, DateTimeKind.Local).AddTicks(4880)),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 622, DateTimeKind.Local).AddTicks(5799))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 23, 13, 42, 3, 999, DateTimeKind.Local).AddTicks(8557),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 571, DateTimeKind.Local).AddTicks(3430));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ChargerGroups",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 2, 23, 13, 42, 3, 995, DateTimeKind.Local).AddTicks(8664),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 2, 27, 11, 30, 39, 567, DateTimeKind.Local).AddTicks(4464));
        }
    }
}
