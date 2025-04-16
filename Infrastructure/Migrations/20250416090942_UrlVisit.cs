using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UrlVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "UrlVisits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "UrlVisits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "UrlVisits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "UrlVisits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ShortUrlId",
                table: "UrlVisits",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "UrlVisits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitedAt",
                table: "UrlVisits",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UrlVisits_ShortUrlId",
                table: "UrlVisits",
                column: "ShortUrlId");

            migrationBuilder.AddForeignKey(
                name: "FK_UrlVisits_ShortUrls_ShortUrlId",
                table: "UrlVisits",
                column: "ShortUrlId",
                principalTable: "ShortUrls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrlVisits_ShortUrls_ShortUrlId",
                table: "UrlVisits");

            migrationBuilder.DropIndex(
                name: "IX_UrlVisits_ShortUrlId",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "ShortUrlId",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "UrlVisits");

            migrationBuilder.DropColumn(
                name: "VisitedAt",
                table: "UrlVisits");
        }
    }
}
