using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Books.Api.Migrations
{
    public partial class InitialLibraryMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: false),
                    Description = table.Column<string>(maxLength: 2500, nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "Flann", "O'Brien" },
                    { new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "James", "Joyce" },
                    { new Guid("24810dfc-2d94-4cc7-aab5-cdf98b83f0c9"), "G.K.", "Chesterton" },
                    { new Guid("2902b665-1190-4c70-9915-b9c2d7680450"), "Bernard", "Shaw" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "The Third Policeman has a fantastic plot of a murderous protagonist let loose on a strange world peopled by fat policemen, played against a satire of academic debate on an eccentric philosopher called De Selby. Sergeant Pluck introduces the atomic theory of the bicycle.", "The Third Policeman" },
                    { new Guid("d8663e5e-7494-4f81-8739-6e0de1bea7ee"), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "At Swim-Two-Birds works entirely with borrowed (and stolen) characters from other fiction and legend, on the grounds that there are already far too many existing fictional characters.", "At Swim-Two-Birds" },
                    { new Guid("d173e20d-159e-4127-9ce9-b0ac2564ad97"), new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "A Künstlerroman in a modernist style, this novel traces the religious and intellectual awakening of young Stephen Dedalus, a fictional alter ego of Joyce and an allusion to Daedalus, the consummate craftsman of Greek mythology.", "A Portrait of the Artist as a Young Man" },
                    { new Guid("493c3228-3444-4a49-9cc0-e8532edc59b2"), new Guid("24810dfc-2d94-4cc7-aab5-cdf98b83f0c9"), "The book is sometimes referred to as a metaphysical thriller. The work is prefixed with a poem written to Edmund Clerihew Bentley, revisiting the pair's early history and the challenges presented to their early faith by the times.", "The Man Who Was Thursday" },
                    { new Guid("40ff5488-fdab-45b5-bc3a-14302d59869a"), new Guid("2902b665-1190-4c70-9915-b9c2d7680450"), "Misalliance is an ironic examination of the mating instincts of a varied group of people gathered at a wealthy man's country home on a summer weekend.", "Misalliance" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
