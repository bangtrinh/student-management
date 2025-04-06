using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Policy = "RequireAdminRole")]

    public class ClassController : Controller
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMajorRepository _majorRepository;

        public ClassController(IClassRepository classRepository, ITeacherRepository teacherRepository, IMajorRepository majorRepository)
        {
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
            _majorRepository = majorRepository;
        }

        public IActionResult Index()
        {
            var teachers = _teacherRepository.GetAll();
            var majors = _majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName");
            var classes = _classRepository.GetAll();
            return View(classes);
        }

        public IActionResult Details(string id)
        {
            var classEntity = _classRepository.GetById(id);
            if (classEntity == null) return NotFound();
            return View(classEntity);
        }

        public IActionResult Create()
        {
            var teachers = _teacherRepository.GetAll();
            var majors =_majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Class classEntity)
        {
            if (ModelState.IsValid)
            {
                _classRepository.Add(classEntity);
                return RedirectToAction(nameof(Index));
            }
            return View(classEntity);
        }

        public IActionResult Edit(string id)
        {

            var classEntity = _classRepository.GetById(id);
            if (classEntity == null) return NotFound();
            var teachers = _teacherRepository.GetAll();
            var majors = _majorRepository.GetAll();
            ViewBag.Majors = new SelectList(majors, "MajorID", "MajorName");
            ViewBag.Teachers = new SelectList(teachers, "TeacherID", "FullName");
            return View(classEntity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Class classEntity)
        {
            if (id != classEntity.ClassID) return BadRequest();
            if (ModelState.IsValid)
            {
                _classRepository.Update(classEntity);
                return RedirectToAction(nameof(Index));
            }
            return View(classEntity);
        }

        public IActionResult Delete(string id)
        {
            var classEntity = _classRepository.GetById(id);
            if (classEntity == null) return NotFound();
            return View(classEntity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _classRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}