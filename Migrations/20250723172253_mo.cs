using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakanat.Migrations
{
    /// <inheritdoc />
    public partial class mo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumper",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumper",
                table: "Apartments");
        }
    }
}
