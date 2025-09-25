using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoeCartBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCartItemProductNav : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Products_ProductId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ImageMimeType",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "CartItem");

            migrationBuilder.AlterColumn<string>(
                name: "ImageMimeType",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageData",
                table: "CartItem",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Carts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageMimeType",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImageMimeType",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "ImageData",
                table: "CartItem",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CartItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "CartItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CartItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "CartItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "CartItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductId",
                table: "Carts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Products_ProductId",
                table: "Carts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
