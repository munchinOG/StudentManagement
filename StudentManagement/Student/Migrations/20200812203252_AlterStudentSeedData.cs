using Microsoft.EntityFrameworkCore.Migrations;

namespace Student.Migrations
{
    public partial class AlterStudentSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "munchin@ut.com");

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Department", "Email", "Name" },
                values: new object[] { 2, 2, "tom@ut.com", "Tom" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "Munchin@ut.com");
        }
    }
}
