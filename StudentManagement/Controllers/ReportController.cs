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

                // Phân trang
                var pagedSummaries = summaries.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var totalCount = summaries.Count;

                ViewBag.TotalCount = totalCount;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.AcademicYear = academicYear;
                ViewBag.Semester = semester;
                ViewBag.ClassId = classId;
                ViewBag.MajorId = majorId;

                var classes = _classRepository.GetAll();
                var majors = _majorRepository.GetAll();
                ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
                ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");

                return View("GradeReport", pagedSummaries);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);

                var classes = _classRepository.GetAll();
                var majors = _majorRepository.GetAll();
                ViewBag.Classes = new SelectList(classes, "ClassID", "ClassName");
                ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");

                return View("GradeReport");
            }
        }

        public IActionResult ExportToExcel(
            string academicYear,
            string semester,
            string classId,
            string majorId)
        {
            var summaries = _gradeService.GetStudentGradeSummaries(academicYear, semester, classId, majorId);

            var excelBytes = ExcelHelper.GenerateGradeReportExcel(summaries);
            return File(excelBytes,
                      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                      $"StudentGradeReport_{academicYear}_{semester}.xlsx");
        }
    }
}