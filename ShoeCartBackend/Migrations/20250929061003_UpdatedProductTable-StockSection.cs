using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeCartBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProductTableStockSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "Products");
        }
    }
}
