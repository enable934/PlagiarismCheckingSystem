using Microsoft.EntityFrameworkCore.Migrations;

namespace PlagiarismCheckingSystem.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LaboratoryWork",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Similarity = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryWork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboratoryWork_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    LaboratoryWorkId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_LaboratoryWork_LaboratoryWorkId",
                        column: x => x.LaboratoryWorkId,
                        principalTable: "LaboratoryWork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Similarity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SimilarityToId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    FileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Similarity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Similarity_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_LaboratoryWorkId",
                table: "Files",
                column: "LaboratoryWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryWork_UserId",
                table: "LaboratoryWork",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Similarity_FileId",
                table: "Similarity",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Similarity");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "LaboratoryWork");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
