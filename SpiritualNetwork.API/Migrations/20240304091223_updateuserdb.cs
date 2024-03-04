using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class updateuserdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardName",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "YearOfPassing",
                schema: "dbo",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "SchoolName",
                schema: "dbo",
                table: "Users",
                newName: "Specialization");

            migrationBuilder.RenameColumn(
                name: "Degree",
                schema: "dbo",
                table: "Users",
                newName: "HighestQualification");

            migrationBuilder.RenameColumn(
                name: "CollegeName",
                schema: "dbo",
                table: "Users",
                newName: "Grades");

            migrationBuilder.AlterColumn<string>(
                name: "LoginMethod",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassingYear",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingYear",
                schema: "dbo",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalExperience",
                schema: "dbo",
                table: "Users",
                type: "int",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassingYear",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartingYear",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalExperience",
                schema: "dbo",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                schema: "dbo",
                table: "Users",
                newName: "SchoolName");

            migrationBuilder.RenameColumn(
                name: "HighestQualification",
                schema: "dbo",
                table: "Users",
                newName: "Degree");

            migrationBuilder.RenameColumn(
                name: "Grades",
                schema: "dbo",
                table: "Users",
                newName: "CollegeName");

            migrationBuilder.AlterColumn<string>(
                name: "LoginMethod",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoardName",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearOfPassing",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true);
        }
    }
}
