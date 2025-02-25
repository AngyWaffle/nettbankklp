using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nettbank.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransaction2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Transaction",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Transaction",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
