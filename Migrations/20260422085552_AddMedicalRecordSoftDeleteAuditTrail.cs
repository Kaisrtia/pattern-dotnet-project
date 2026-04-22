using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pattern_project.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecordSoftDeleteAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "MedicalRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "MedicalRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MedicalRecords");
        }
    }
}
