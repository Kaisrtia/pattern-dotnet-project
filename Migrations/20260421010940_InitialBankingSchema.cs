using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace pattern_project.Migrations
{
    /// <inheritdoc />
    public partial class InitialBankingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.CheckConstraint("CK_Accounts_Balance", "[Balance] >= 0");
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OwnerUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.CheckConstraint("CK_Transactions_Amount", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterbankTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceAccountId = table.Column<long>(type: "bigint", nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    DestinationBankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DestinationAccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DigitalSignature = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresDigitalSignature = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterbankTransactions", x => x.Id);
                    table.CheckConstraint("CK_InterbankTransactions_DigitalSignature", "[RequiresDigitalSignature] = 0 OR [DigitalSignature] IS NOT NULL");
                    table.CheckConstraint("CK_InterbankTransactions_SwiftCode", "LEN([SwiftCode]) IN (8, 11)");
                    table.ForeignKey(
                        name: "FK_InterbankTransactions_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterbankTransactions_Transactions_Id",
                        column: x => x.Id,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InternalTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceAccountId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationAccountId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalTransactions", x => x.Id);
                    table.CheckConstraint("CK_InternalTransactions_SourceDestination", "[SourceAccountId] <> [DestinationAccountId]");
                    table.ForeignKey(
                        name: "FK_InternalTransactions_Accounts_DestinationAccountId",
                        column: x => x.DestinationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransactions_Accounts_SourceAccountId",
                        column: x => x.SourceAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransactions_Transactions_Id",
                        column: x => x.Id,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "IsDeleted", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), "admin@bank.local", "System Administrator", true, false, "E86F78A8A3CAF0B60D8E74E5942AA6D86DC150CD3C03338AEF25B7D2D7E3ACC7", "Admin", "admin" },
                    { 2L, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), "alice@bank.local", "Alice User", true, false, "3E7C19576488862816F13B512CACF3E4BA97DD97243EA0BD6A2AD1642D86BA72", "User", "alice" },
                    { 3L, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), "bob@bank.local", "Bob User", true, false, "3E7C19576488862816F13B512CACF3E4BA97DD97243EA0BD6A2AD1642D86BA72", "User", "bob" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "Balance", "CreatedAt", "IsDeleted", "UserId" },
                values: new object[,]
                {
                    { 1L, "970400000001", 500000000m, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), false, 2L },
                    { 2L, "970400000002", 300000000m, new DateTime(2026, 4, 21, 0, 0, 0, 0, DateTimeKind.Utc), false, 3L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountNumber",
                table: "Accounts",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterbankTransactions_SourceAccountId",
                table: "InterbankTransactions",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InterbankTransactions_SwiftCode",
                table: "InterbankTransactions",
                column: "SwiftCode");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransactions_DestinationAccountId",
                table: "InternalTransactions",
                column: "DestinationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransactions_SourceAccountId",
                table: "InternalTransactions",
                column: "SourceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OwnerUserId_IsDeleted_CreatedAt",
                table: "Transactions",
                columns: new[] { "OwnerUserId", "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
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
                name: "InterbankTransactions");

            migrationBuilder.DropTable(
                name: "InternalTransactions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
