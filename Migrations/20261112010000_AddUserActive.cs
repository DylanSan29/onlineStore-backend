using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineStoreBackend.Data;

#nullable disable

namespace OnlineStoreBackend.Migrations;

[DbContext(typeof(OnlineStoreContext))]
[Migration("20261112010000_AddUserActive")]
public partial class AddUserActive : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.AddColumn<bool>(
        name: "IsActive", table: "Users", type: "bit", nullable: false, defaultValue: true);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropColumn(name: "IsActive", table: "Users");
}
