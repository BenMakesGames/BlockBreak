using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlockBreak.Migrations
{
    /// <inheritdoc />
    public partial class InitialHighScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HighScoreEntries",
                columns: new[] { "Id", "CreatedOn", "Name", "Score" },
                values: new object[,]
                {
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b0"), null, "OMG", 161250L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b1"), null, "WOW", 146300L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b2"), null, "BEN", 132500L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b3"), null, "ENM", 117750L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b4"), null, "VER", 103500L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b5"), null, "ROY", 89250L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b6"), null, "OFT", 75000L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b7"), null, "BBQ", 60750L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b8"), null, "LOL", 46500L },
                    { new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b9"), null, "AFK", 32250L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b0"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b1"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b2"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b3"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b4"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b5"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b6"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b7"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b8"));

            migrationBuilder.DeleteData(
                table: "HighScoreEntries",
                keyColumn: "Id",
                keyValue: new Guid("2d75a0f4-bac1-41b2-811d-20e7d6ee96b9"));
        }
    }
}
