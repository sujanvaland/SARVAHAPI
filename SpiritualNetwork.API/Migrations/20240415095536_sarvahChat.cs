﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiritualNetwork.API.Migrations
{
    /// <inheritdoc />
    public partial class sarvahChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    ReplyId = table.Column<int>(type: "int", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDelivered = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<int>(type: "int", nullable: true),
                    AttachmentId = table.Column<int>(type: "int", nullable: true),
                    DeleteForUserId1 = table.Column<int>(type: "int", nullable: true),
                    DeleteForUserId2 = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recuiter",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecuiterId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoOfEmployee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WebsiteLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recuiter", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Recuiter",
                schema: "dbo");
        }
    }
}
