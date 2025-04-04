using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]

    public class MajorController : Controller
    {
        private readonly IMajorRepository _majorRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public MajorController(IMajorRepository majorRepository, IDepartmentRepository departmentRepository)
        {
            _majorRepository = majorRepository;
            _departmentRepository = departmentRepository;
        }

        public IActionResult Index()
        {
            var majors = _majorRepository.GetAll();
            return View(majors);
        }

        public IActionResult Details(string id)
        {
            var major = _majorRepository.GetById(id);
            if (major == null) return NotFound();
            return View(major);
        }

        public IActionResult Create()
        {
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Major major)
        {
            if (!ModelState.IsValid)
            {
                _majorRepository.Add(major);
                return RedirectToAction(nameof(Index));
            }
            return View(major);
        }

        public IActionResult Edit(string id)
        {
            var major = _majorRepository.GetById(id);
            if (major == null) return NotFound();
            var departments = _departmentRepository.GetAll();
            ViewBag.Departments = new SelectList(departments, "DepartmentID", "DepartmentName");
            return View(major);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Major major)
        {
            if (id != major.MajorID) return BadRequest();
            if (!ModelState.IsValid)
            {
                _majorRepository.Update(major);
                return RedirectToAction(nameof(Index));
            }
            return View(major);
        }

        public IActionResult Delete(string id)
        {
            var major = _majorRepository.GetById(id);
            if (major == null) return NotFound();
            return View(major);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _majorRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}