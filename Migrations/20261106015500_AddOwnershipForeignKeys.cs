using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineStoreBackend.Data;

#nullable disable

namespace OnlineStoreBackend.Migrations;

[DbContext(typeof(OnlineStoreContext))]
[Migration("20261106015500_AddOwnershipForeignKeys")]
public partial class AddOwnershipForeignKeys : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(name: "FK_Orders_Users_UserId", table: "Orders", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.AddForeignKey(name: "FK_OrderItems_Products_ProductId", table: "OrderItems", column: "ProductId", principalTable: "Products", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.AddForeignKey(name: "FK_InventoryMovements_Products_ProductId", table: "InventoryMovements", column: "ProductId", principalTable: "Products", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.AddForeignKey(name: "FK_Addresses_Users_UserId", table: "Addresses", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Orders_Users_UserId", table: "Orders");
        migrationBuilder.DropForeignKey(name: "FK_OrderItems_Products_ProductId", table: "OrderItems");
        migrationBuilder.DropForeignKey(name: "FK_InventoryMovements_Products_ProductId", table: "InventoryMovements");
        migrationBuilder.DropForeignKey(name: "FK_Addresses_Users_UserId", table: "Addresses");
    }
}
