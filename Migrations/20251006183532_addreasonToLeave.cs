using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class addreasonToLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Leaves");
        }
    }
}
