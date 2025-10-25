using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeCartBackend.Migrations
{
    /// <inheritdoc />
    public partial class TablesUpdatedFromBase64ToCloudnary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ImageMimeType",
                table: "ProductImage",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImage",
                newName: "ImageMimeType");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "ProductImage",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "OrderItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "CartItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
