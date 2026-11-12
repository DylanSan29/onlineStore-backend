using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineStoreBackend.Data;

#nullable disable

namespace OnlineStoreBackend.Migrations;

[DbContext(typeof(OnlineStoreContext))]
[Migration("20261106014000_AddInventoryMovements")]
public partial class AddInventoryMovements : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "InventoryMovements",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ProductId = table.Column<int>(type: "int", nullable: false),
                QuantityChanged = table.Column<int>(type: "int", nullable: false),
                MovementType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_InventoryMovements", movement => movement.Id));

        migrationBuilder.CreateIndex("IX_InventoryMovements_ProductId", "InventoryMovements", "ProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("InventoryMovements");
}
