// Helpers/DateHelper.cs
namespace StudentManagement.Helper
{
    public static class DateHelper
    {
        public static (string Semester, string AcademicYear) GetCurrentSemesterAndAcademicYear()
        {
            var currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int month = currentDate.Month;
            string semester;
            string academicYear;
            // Xác định học kỳ dựa trên tháng
            if (month >= 9 && month <= 12)
            {
                semester = "HK1";
                academicYear = $"{currentYear}-{currentYear + 1}";
            }
            else if (month >= 1 && month <= 4)
            {
                semester = "HK2";
                academicYear = $"{currentYear - 1}-{currentYear}";
            }
            else // Tháng 5-8
            {
                semester = "HK3";
                academicYear = $"{currentYear - 1}-{currentYear}";
            }
            return (semester, academicYear);
        }
    }
}