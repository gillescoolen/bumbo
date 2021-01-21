using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bumbo.Data.Migrations
{
    public partial class Scaffolded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActualTimeWorked",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    WorkDate = table.Column<DateTime>(type: "date", nullable: false),
                    Start = table.Column<TimeSpan>(nullable: false),
                    Finish = table.Column<TimeSpan>(nullable: false),
                    Sickness = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actual_Time_Worked", x => new { x.WorkDate, x.UserId });
                    table.ForeignKey(
                        name: "FK_Actual_Time_Worked_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvailableWorktime",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    WorkDate = table.Column<DateTime>(type: "date", nullable: false),
                    Start = table.Column<TimeSpan>(nullable: false),
                    Finish = table.Column<TimeSpan>(nullable: false),
                    SchoolHoursWorked = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Available_Worktime", x => new { x.WorkDate, x.UserId });
                    table.ForeignKey(
                        name: "FK_Available_Worktime_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostalCode = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    HouseNumber = table.Column<int>(nullable: false),
                    HouseNumberLetter = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    StreetName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FurloughRequest",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    WorkDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsApproved = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Furlough_Request", x => new { x.WorkDate, x.UserId });
                    table.ForeignKey(
                        name: "FK_Furlough_Request_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlannedWorktime",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    workDate = table.Column<DateTime>(type: "date", nullable: false),
                    Start = table.Column<TimeSpan>(nullable: false),
                    Finish = table.Column<TimeSpan>(nullable: false),
                    Section = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planned_Worktime", x => new { x.workDate, x.UserId });
                    table.ForeignKey(
                        name: "FK_Planned_Worktime_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    tokenID = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.tokenID);
                    table.ForeignKey(
                        name: "FK_Token_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Norm",
                columns: table => new
                {
                    Activity = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    Norm = table.Column<int>(nullable: false),
                    NormDescription = table.Column<string>(unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Norm", x => new { x.Activity, x.BranchId });
                    table.ForeignKey(
                        name: "FK_Norm_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prognoses",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    AmountOfCustomers = table.Column<int>(nullable: false),
                    AmountOfFreight = table.Column<int>(nullable: false),
                    WeatherDescription = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prognoses", x => new { x.Date, x.BranchId });
                    table.ForeignKey(
                        name: "FK_Prognoses_Branch",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchId",
                table: "Users",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualTimeWorked_UserId",
                table: "ActualTimeWorked",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableWorktime_UserId",
                table: "AvailableWorktime",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FurloughRequest_UserId",
                table: "FurloughRequest",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Norm_BranchId",
                table: "Norm",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedWorktime_UserId",
                table: "PlannedWorktime",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Prognoses_BranchId",
                table: "Prognoses",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_UserId",
                table: "Token",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branch_BranchId",
                table: "Users",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Branch_BranchId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ActualTimeWorked");

            migrationBuilder.DropTable(
                name: "AvailableWorktime");

            migrationBuilder.DropTable(
                name: "FurloughRequest");

            migrationBuilder.DropTable(
                name: "Norm");

            migrationBuilder.DropTable(
                name: "PlannedWorktime");

            migrationBuilder.DropTable(
                name: "Prognoses");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropIndex(
                name: "IX_Users_BranchId",
                table: "Users");
        }
    }
}
