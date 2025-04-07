using StudentManagement.Models.ViewModel;
using StudentManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Repositories;
using StudentManagement.Helpers;
using StudentManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Policy = "RequireAdminRole")]
    public class ReportController : Controller
    {
        private readonly IGradeService _gradeService;
        private readonly IClassRepository _classRepository;
        private readonly IMajorRepository _majorRepository;

        public ReportController(IGradeService gradeService, IClassRepository classRepository, IMajorRepository majorRepository)
        {
            _gradeService = gradeService;
            _classRepository = classRepository;
            _majorRepository = majorRepository;
        }

        [HttpGet]
        public IActionResult GradeReport()
        {
            var classes = _classRepository.GetAll();
            var majors = _majorRepository.GetAll();
            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");
            return View();
        }

        [HttpPost]
        public IActionResult GradeReport(
    string academicYear,
    string semester,
    string classId,
    string majorId,
    int page = 1,
    int pageSize = 10)
        {
            try
            {
                var summaries = _gradeService.GetStudentGradeSummaries(academicYear, semester, classId, majorId);

                var pagedSummaries = summaries
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.TotalCount = summaries.Count;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.AcademicYear = academicYear;
                ViewBag.Semester = semester;
                ViewBag.ClassId = classId;
                ViewBag.MajorId = majorId;

                LoadDropdowns();

                return View("GradeReport", pagedSummaries);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.AcademicYear = academicYear;
                ViewBag.Semester = semester;
                ViewBag.ClassId = classId;
                ViewBag.MajorId = majorId;

                LoadDropdowns();

                return View("GradeReport", new List<StudentGradeSummary>());
            }
        }

        private void LoadDropdowns()
        {
            var classes = _classRepository.GetAll();
            var majors = _majorRepository.GetAll();

            ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");
        }


        public IActionResult ExportToExcel(
    string academicYear,
    string semester,
    string classId,
    string majorId)
        {
            try
            {
                var summaries = _gradeService.GetStudentGradeSummaries(academicYear, semester, classId, majorId);

                if (summaries == null || !summaries.Any())
                {
                    return BadRequest("Không có dữ liệu để xuất file Excel.");
                }

                var excelBytes = ExcelHelper.GenerateGradeReportExcel(summaries);

                var fileName = $"StudentGradeReport_{academicYear ?? "AllYears"}_{semester ?? "AllSemesters"}_{classId ?? "AllClasses"}_{majorId ?? "AllMajors"}.xlsx";

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}