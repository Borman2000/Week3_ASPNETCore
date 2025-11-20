using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedemailtoAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                table: "Authors",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Authors",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d1"),
                columns: new[] { "BirthDate", "Email" },
                values: new object[] { new DateOnly(1920, 1, 2), null });

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d2"),
                columns: new[] { "BirthDate", "Email" },
                values: new object[] { new DateOnly(1920, 8, 22), null });

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d3"),
                columns: new[] { "BirthDate", "Email" },
                values: new object[] { new DateOnly(1907, 7, 7), null });

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d4"),
                columns: new[] { "BirthDate", "Email" },
                values: new object[] { new DateOnly(1947, 9, 21), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Authors");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Authors",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d1"),
                column: "BirthDate",
                value: new DateTime(1920, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d2"),
                column: "BirthDate",
                value: new DateTime(1920, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d3"),
                column: "BirthDate",
                value: new DateTime(1907, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("ab29fc40-ca47-1067-b31d-00dd010662d4"),
                column: "BirthDate",
                value: new DateTime(1947, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
