using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class salarynew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxSalary",
                schema: "dbo",
                table: "JobPost",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinSalary",
                schema: "dbo",
                table: "JobPost",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSalary",
                schema: "dbo",
                table: "JobPost");

            migrationBuilder.DropColumn(
                name: "MinSalary",
                schema: "dbo",
                table: "JobPost");
        }
    }
}
