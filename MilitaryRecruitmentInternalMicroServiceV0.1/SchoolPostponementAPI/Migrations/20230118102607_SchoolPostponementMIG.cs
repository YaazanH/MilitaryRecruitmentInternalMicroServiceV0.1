﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SchoolPostponementAPI.Migrations
{
    public partial class SchoolPostponementMIG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RabbitMQobjDBS",
                columns: table => new
                {
                    RequestStatuseID = table.Column<int>(type: "int", nullable: false),
                    ProcID = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JWT = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RabbitMQResponceDBS",
                columns: table => new
                {
                    RequestStatuseID = table.Column<int>(type: "int", nullable: false),
                    ProcID = table.Column<int>(type: "int", nullable: false),
                    Responce = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RequestStatuesDBS",
                columns: table => new
                {
                    ReqStatuesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PostponmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfRecive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfDone = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuesDBS", x => x.ReqStatuesID);
                });

            migrationBuilder.CreateTable(
                name: "schoolDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateOfEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schoolDBS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserInfoDBS",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JWT = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "AsyncAgeDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    statuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncAgeDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncAgeDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncDroppedOutDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDroppedOut = table.Column<bool>(type: "bit", nullable: false),
                    statuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncDroppedOutDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncDroppedOutDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncStudyingNowDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudyingNow = table.Column<bool>(type: "bit", nullable: false),
                    statuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncStudyingNowDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncStudyingNowDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncStudyYearsDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudyYears = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    statuse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncStudyYearsDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncStudyYearsDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsyncAgeDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncAgeDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncDroppedOutDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncDroppedOutDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncStudyingNowDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncStudyingNowDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncStudyYearsDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncStudyYearsDBS",
                column: "RequestStatuesIDReqStatuesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsyncAgeDBS");

            migrationBuilder.DropTable(
                name: "AsyncDroppedOutDBS");

            migrationBuilder.DropTable(
                name: "AsyncStudyingNowDBS");

            migrationBuilder.DropTable(
                name: "AsyncStudyYearsDBS");

            migrationBuilder.DropTable(
                name: "RabbitMQobjDBS");

            migrationBuilder.DropTable(
                name: "RabbitMQResponceDBS");

            migrationBuilder.DropTable(
                name: "schoolDBS");

            migrationBuilder.DropTable(
                name: "UserInfoDBS");

            migrationBuilder.DropTable(
                name: "RequestStatuesDBS");
        }
    }
}
