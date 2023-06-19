using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreateExcelFile.Migrations
{
    /// <inheritdoc />
    public partial class za : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileStatus",
                table: "UserFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileStatus",
                table: "UserFiles");
        }
    }
}
