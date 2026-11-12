using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineStoreBackend.Data;

#nullable disable

namespace OnlineStoreBackend.Migrations;

[DbContext(typeof(OnlineStoreContext))]
[Migration("20261112020000_AddPayments")]
public partial class AddPayments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                OrderId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProviderPaymentId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", payment => payment.Id);
                table.ForeignKey("FK_Payments_Orders_OrderId", payment => payment.OrderId, "Orders", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Payments_Users_UserId", payment => payment.UserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(name: "IX_Payments_IdempotencyKey", table: "Payments", column: "IdempotencyKey", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Payments_ProviderPaymentId", table: "Payments", column: "ProviderPaymentId", unique: true, filter: "[ProviderPaymentId] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Payments");
}
