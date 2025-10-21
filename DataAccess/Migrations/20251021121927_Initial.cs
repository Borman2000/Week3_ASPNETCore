using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FirstName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Biography = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AuthorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ISBN = table.Column<string>(type: "varchar(17)", maxLength: 17, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookCategory",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookCategory", x => new { x.BookId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_BookCategory_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("ab29fc40-ca47-1067-b31d-00dd010662d1"), null, new DateTime(1920, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Isaac", "Asimov" },
                    { new Guid("ab29fc40-ca47-1067-b31d-00dd010662d2"), null, new DateTime(1920, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ray", "Bradbury" },
                    { new Guid("ab29fc40-ca47-1067-b31d-00dd010662d3"), "Robert Anson Heinlein (July 7, 1907 – May 8, 1988) was an American science fiction author, aeronautical engineer, and naval officer. Sometimes called the \"dean of science fiction writers\", he was among the first to emphasize scientific accuracy in his fiction and was thus a pioneer of the subgenre of hard science fiction. His published works, both fiction and non-fiction, express admiration for competence and emphasize the value of critical thinking. His plots often posed provocative situations which challenged conventional social mores. His work continues to have an influence on the science-fiction genre and on modern culture more generally.", new DateTime(1907, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robert", "Heinlein" },
                    { new Guid("ab29fc40-ca47-1067-b31d-00dd010662d4"), null, new DateTime(1947, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stephen", "King" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e751"), null, "Dystopian" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e752"), null, "Fantasy" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e753"), null, "Classics" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e754"), null, "Mystery" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e755"), null, "Horror" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e756"), "The genre of speculative fiction that imagines advanced and futuristic scientific progress and typically includes elements like information technology and robotics, biological manipulations, space exploration, time travel, parallel universes, and extraterrestrial life.", "Science Fiction" },
                    { new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e757"), null, "Military" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "ISBN", "Price", "Title", "Year" },
                values: new object[,]
                {
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c01"), new Guid("ab29fc40-ca47-1067-b31d-00dd010662d2"), "978-0-7432-4722-1", 9.99m, "Fahrenheit 451", 1953 },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c02"), new Guid("ab29fc40-ca47-1067-b31d-00dd010662d2"), "0-553-27753-7", 7.99m, "Dandelion Wine", 1957 },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c03"), new Guid("ab29fc40-ca47-1067-b31d-00dd010662d4"), "978-0451223296", 5.99m, "The Mist", 1976 },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c04"), new Guid("ab29fc40-ca47-1067-b31d-00dd010662d1"), "0-385-02701-X", 8.99m, "The Gods Themselves", 1972 },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c05"), new Guid("ab29fc40-ca47-1067-b31d-00dd010662d3"), "978-0450044496", 5.99m, "Starship Troopers", 1959 }
                });

            migrationBuilder.InsertData(
                table: "BookCategory",
                columns: new[] { "BookId", "CategoryId" },
                values: new object[,]
                {
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c01"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e751") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c02"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e752") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c02"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e753") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c02"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e756") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c03"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e755") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c03"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e756") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c04"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e756") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c05"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e756") },
                    { new Guid("be61d971-5ebc-4f02-a3a9-6c82895e5c05"), new Guid("c4cd02b7-5d56-403d-9041-cc4f3851e757") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FirstName_LastName",
                table: "Authors",
                columns: new[] { "FirstName", "LastName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookCategory_CategoryId",
                table: "BookCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title",
                table: "Books",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookCategory");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
