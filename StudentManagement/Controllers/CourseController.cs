using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly ITeacherRepository _teacherRepository;

        public CourseController(ICourseRepository courseRepository, IStudentRepository studentRepository, ITeacherRepository teacherRepository, IMajorRepository majorRepository)
        {
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _majorRepository = majorRepository;
        }

        [Authorize(Policy = "RequireAdminRole")]



        [Authorize(Policy = "RequireAdminOrTeacher")]

        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var course = _courseRepository.GetCourseDetails(id);
            if (course == null) return NotFound();

            return View(course);
        }

        [Authorize(Policy = "RequireAdminRole")]

        public IActionResult Create()
        {
            var majors = _majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");

            var teachers = _teacherRepository.GetAll();
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName");
            return View();
        }

        [Authorize(Policy = "RequireAdminRole")]


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _courseRepository.Add(course);
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [Authorize(Policy = "RequireAdminRole")]

        public async Task<IActionResult> Edit(string id)
        {
            var course = _courseRepository.GetById(id);
            if (course == null)
            {
                return NotFound();
            }

            var majors = _majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName", course.MajorID);

            var teachers = _teacherRepository.GetAll();
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName", course.TeacherID);

            return View(course);
        }

        [Authorize(Policy = "RequireAdminRole")]
        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _courseRepository.Update(course);
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không valid, reload danh sách Majors và Teachers
            var majors = _majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName", course.MajorID);

            var teachers = _teacherRepository.GetAll();
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName", course.TeacherID);

            return View(course);
        }

        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Delete(string id)
        {
            var course = _courseRepository.GetById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _courseRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        //Tìm kiếm
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Index(string searchString)
        {
            var courses = _courseRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.CourseID.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               c.CourseName.Contains(searchString, StringComparison.OrdinalIgnoreCase));

            }

            return View(courses);
        }

        }
    }