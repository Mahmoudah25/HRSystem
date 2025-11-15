using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class addisdelAnnonucemt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Announcements");
        }
    }
}
