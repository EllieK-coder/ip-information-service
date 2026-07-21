using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_IpAddress_CountryTwoLetterCode_LastUpdated",
                table: "Ipv4Address",
                newName: "IX_Ipv4Address_TwoLetterCode_LastUpdated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Ipv4Address_TwoLetterCode_LastUpdated",
                table: "Ipv4Address",
                newName: "IX_IpAddress_CountryTwoLetterCode_LastUpdated");
        }
    }
}
