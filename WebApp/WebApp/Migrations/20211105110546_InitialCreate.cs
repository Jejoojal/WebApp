using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    etunimi = table.Column<string>(nullable: true),
                    sukunimi = table.Column<string>(nullable: true),
                    sposti = table.Column<string>(nullable: true),
                    puhelin = table.Column<string>(nullable: true),
                    katuosoite = table.Column<string>(nullable: true),
                    postinumero = table.Column<string>(nullable: true),
                    postitoimipaikka = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profile");
        }
    }
}
