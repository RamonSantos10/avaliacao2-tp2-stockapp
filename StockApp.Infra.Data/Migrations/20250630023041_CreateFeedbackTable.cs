using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockApp.Infra.Data.Migrations
{
    public partial class CreateFeedbackTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
            name: "Feedbacks",
            columns: table => new
    {
            Id = table.Column<int>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
            UserId = table.Column<string>(nullable: true),
            FeedbackText = table.Column<string>(nullable: true),
            Sentiment = table.Column<string>(nullable: true),
            CreatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
              table.PrimaryKey("PK_Feedbacks", x => x.Id);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
