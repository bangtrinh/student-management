using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Migrations
{
    /// <inheritdoc />
    public partial class updatemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "Grades",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            // Xóa khóa chính nếu cần
            migrationBuilder.DropPrimaryKey(name: "PK_Grades", table: "Grades");

            // Xóa cột GradeID cũ
            migrationBuilder.DropColumn(name: "GradeID", table: "Grades");

            // Thêm lại GradeID với IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "GradeID",
                table: "Grades",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // Đặt lại khóa chính
            migrationBuilder.AddPrimaryKey(name: "PK_Grades", table: "Grades", column: "GradeID");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Courses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CourseID = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ClassDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK_Schedules_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Email",
                table: "Teachers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CourseID",
                table: "Schedules",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_StudentID",
                table: "Schedules",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_Email",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<float>(
                name: "Score",
                table: "Grades",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            // Xóa khóa chính
            migrationBuilder.DropPrimaryKey(name: "PK_Grades", table: "Grades");

            // Xóa cột GradeID mới
            migrationBuilder.DropColumn(name: "GradeID", table: "Grades");

            // Thêm lại GradeID như cũ (không có IDENTITY)
            migrationBuilder.AddColumn<string>(
                name: "GradeID",
                table: "Grades",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false);

            // Đặt lại khóa chính
            migrationBuilder.AddPrimaryKey(name: "PK_Grades", table: "Grades", column: "GradeID");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Classes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}