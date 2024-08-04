using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressTracker.Migrations
{
    /// <inheritdoc />
    public partial class addedDailyRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetTicks = table.Column<long>(type: "bigint", nullable: false),
                    BreakTicks = table.Column<long>(type: "bigint", nullable: false),
                    SessionIdsString = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyRecords_UserId_Date",
                table: "DailyRecords",
                columns: new[] { "UserId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyRecords");
        }
    }
}
