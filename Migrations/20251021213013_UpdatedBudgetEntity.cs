using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBudgetEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Budgets");

            migrationBuilder.AlterColumn<decimal>(
                name: "Limit",
                table: "Budgets",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Limit",
                table: "Budgets",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Budgets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
