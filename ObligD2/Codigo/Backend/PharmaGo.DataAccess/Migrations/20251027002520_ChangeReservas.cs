using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaGo.DataAccess.Migrations
{
    public partial class ChangeReservas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Confirmada",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EmpleadoConfirmacion",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacion",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaExpiracion",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaReserva",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "RequierePrescripcion",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TienePrescripcion",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmada",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "EmpleadoConfirmacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaExpiracion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "FechaReserva",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "RequierePrescripcion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "TienePrescripcion",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Reservas");
        }
    }
}
