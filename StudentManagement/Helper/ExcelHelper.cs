using StudentManagement.Models.ViewModel;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml.Style;

namespace StudentManagement.Helpers
{
    public static class ExcelHelper
    {
        public static byte[] GenerateGradeReportExcel(List<StudentGradeSummary> summaries)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Grade Report");

                // Thêm tiêu đề
                worksheet.Cells[1, 1].Value = "Mã SV";
                worksheet.Cells[1, 2].Value = "Họ và tên";
                worksheet.Cells[1, 3].Value = "Lớp";
                worksheet.Cells[1, 4].Value = "Ngành";
                worksheet.Cells[1, 5].Value = "Năm học";
                worksheet.Cells[1, 6].Value = "Học kỳ";
                worksheet.Cells[1, 7].Value = "Điểm TB";

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Thêm dữ liệu
                for (int i = 0; i < summaries.Count; i++)
                {
                    var row = i + 2;
                    worksheet.Cells[row, 1].Value = summaries[i].StudentID;
                    worksheet.Cells[row, 2].Value = summaries[i].FullName;
                    worksheet.Cells[row, 3].Value = summaries[i].ClassName;
                    worksheet.Cells[row, 4].Value = summaries[i].MajorName;
                    worksheet.Cells[row, 5].Value = summaries[i].AcademicYear;
                    worksheet.Cells[row, 6].Value = summaries[i].Semester;
                    worksheet.Cells[row, 7].Value = summaries[i].AverageScore;

                    // Định dạng số
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "0";
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "0.00";
                }

                // Tự động điều chỉnh độ rộng cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}