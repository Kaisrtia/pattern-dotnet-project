using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace pattern_project.Migrations
{
    /// <inheritdoc />
    public partial class InitMedicalRecordTpt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExaminationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsDangerousInfectiousDisease = table.Column<bool>(type: "bit", nullable: false),
                    MedicalVerificationCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.CheckConstraint("CK_MedicalRecords_VerificationCode_WhenInfectious", "([IsDangerousInfectiousDisease] = 0 AND [MedicalVerificationCode] IS NULL) OR ([IsDangerousInfectiousDisease] = 1 AND [MedicalVerificationCode] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InpatientMedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BedNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InpatientMedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InpatientMedicalRecords_MedicalRecords_Id",
                        column: x => x.Id,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutpatientMedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EPrescriptionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutpatientMedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutpatientMedicalRecords_MedicalRecords_Id",
                        column: x => x.Id,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "Admin@123", "Admin", "admin" },
                    { 2, "User@123", "User", "patientA" },
                    { 3, "User@123", "User", "patientB" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId_ExaminationDate",
                table: "MedicalRecords",
                columns: new[] { "PatientId", "ExaminationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_RecordCode",
                table: "MedicalRecords",
                column: "RecordCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InpatientMedicalRecords");

            migrationBuilder.DropTable(
                name: "OutpatientMedicalRecords");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
