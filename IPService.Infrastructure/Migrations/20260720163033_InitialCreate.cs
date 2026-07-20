using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    TwoLetterCode = table.Column<string>(type: "char(2)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThreeLetterCode = table.Column<string>(type: "char(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.TwoLetterCode);
                });

            migrationBuilder.CreateTable(
                name: "Ipv4Address",
                columns: table => new
                {
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "varchar(15)", nullable: false),
                    TwoLetterCode = table.Column<string>(type: "char(2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ipv4Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Ipv4Address_Countries_TwoLetterCode",
                        column: x => x.TwoLetterCode,
                        principalTable: "Countries",
                        principalColumn: "TwoLetterCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_ThreeLetterCode",
                table: "Countries",
                column: "ThreeLetterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IpAddress_CountryTwoLetterCode_LastUpdated",
                table: "Ipv4Address",
                column: "TwoLetterCode")
                .Annotation("SqlServer:Include", new[] { "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Ipv4Address_Address",
                table: "Ipv4Address",
                column: "Address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ipv4Address");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
