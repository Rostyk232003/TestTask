using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConferenceRoomBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HallId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HallServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HallId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HallServices_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HallServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingServices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Halls",
                columns: new[] { "Id", "Capacity", "CreatedAt", "HourlyRate", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("9e27a298-dcae-465d-b028-3766a7825311"), 30, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), 1500m, true, "Hall C" },
                    { new Guid("fa2865e7-39a0-4ea4-abee-0072b6af0253"), 100, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), 3500m, true, "Hall B" },
                    { new Guid("feeecb75-7993-4f76-b8bd-1b3fc3b8b300"), 50, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), 2000m, true, "Hall A" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("0d807f28-2976-4c81-b182-5db457c51819"), "Wi-Fi", 300m },
                    { new Guid("9492a6e0-8ee7-419c-a244-a02e379c2bcd"), "Sound", 700m },
                    { new Guid("b77393a7-a368-49fb-ae51-b43a3c4a4bca"), "Projector", 500m }
                });

            migrationBuilder.InsertData(
                table: "HallServices",
                columns: new[] { "Id", "HallId", "Price", "ServiceId" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("feeecb75-7993-4f76-b8bd-1b3fc3b8b300"), 500m, new Guid("b77393a7-a368-49fb-ae51-b43a3c4a4bca") },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("feeecb75-7993-4f76-b8bd-1b3fc3b8b300"), 300m, new Guid("0d807f28-2976-4c81-b182-5db457c51819") },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("fa2865e7-39a0-4ea4-abee-0072b6af0253"), 500m, new Guid("b77393a7-a368-49fb-ae51-b43a3c4a4bca") },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("fa2865e7-39a0-4ea4-abee-0072b6af0253"), 300m, new Guid("0d807f28-2976-4c81-b182-5db457c51819") },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("fa2865e7-39a0-4ea4-abee-0072b6af0253"), 700m, new Guid("9492a6e0-8ee7-419c-a244-a02e379c2bcd") },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("9e27a298-dcae-465d-b028-3766a7825311"), 300m, new Guid("0d807f28-2976-4c81-b182-5db457c51819") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_HallId",
                table: "Bookings",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_BookingId",
                table: "BookingServices",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_ServiceId",
                table: "BookingServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HallServices_HallId_ServiceId",
                table: "HallServices",
                columns: new[] { "HallId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HallServices_ServiceId",
                table: "HallServices",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingServices");

            migrationBuilder.DropTable(
                name: "HallServices");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Halls");
        }
    }
}
