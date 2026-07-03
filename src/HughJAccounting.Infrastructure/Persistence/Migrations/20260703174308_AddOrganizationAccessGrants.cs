using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HughJAccounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationAccessGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_grants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountingEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    PermissionKey = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExpiresAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RevokedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_grants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_membership_roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationMembershipId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_membership_roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_membership_roles_organization_memberships_Orga~",
                        column: x => x.OrganizationMembershipId,
                        principalTable: "organization_memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_organization_membership_roles_organization_roles_Organizati~",
                        column: x => x.OrganizationRoleId,
                        principalTable: "organization_roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_grants_OrganizationId_AccountingEntityId_ResourceTyp~",
                table: "access_grants",
                columns: new[] { "OrganizationId", "AccountingEntityId", "ResourceType", "ResourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_access_grants_OrganizationId_UserId_PermissionKey_ResourceT~",
                table: "access_grants",
                columns: new[] { "OrganizationId", "UserId", "PermissionKey", "ResourceType", "ResourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_membership_roles_OrganizationMembershipId_Orga~",
                table: "organization_membership_roles",
                columns: new[] { "OrganizationMembershipId", "OrganizationRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_organization_membership_roles_OrganizationRoleId",
                table: "organization_membership_roles",
                column: "OrganizationRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_grants");

            migrationBuilder.DropTable(
                name: "organization_membership_roles");
        }
    }
}
