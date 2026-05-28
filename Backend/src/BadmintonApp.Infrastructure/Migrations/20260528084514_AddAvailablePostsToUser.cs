using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadmintonApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailablePostsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailablePosts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailablePosts",
                table: "Users");
        }
    }
}
