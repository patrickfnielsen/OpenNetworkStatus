using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenNetworkStatus.Data.Migrations
{
    public partial class ControlVisibility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Display",
                table: "Metrics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DecimalPlaces",
                table: "Metrics",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "ExternalMetricIdentifier",
                table: "Metrics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetricProviderType",
                table: "Metrics",
                nullable: false,
                defaultValue: "custom");

            migrationBuilder.AddColumn<bool>(
                name: "Display",
                table: "Components",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Display",
                table: "ComponentGroups",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Display",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "DecimalPlaces",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "ExternalMetricIdentifier",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "MetricProviderId",
                table: "Metrics");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Display",
                table: "ComponentGroups");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2020, 4, 30, 14, 39, 6, 849, DateTimeKind.Utc).AddTicks(3490), new DateTime(2020, 4, 30, 14, 39, 6, 849, DateTimeKind.Utc).AddTicks(2940) });
        }
    }
}
