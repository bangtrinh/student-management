﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.Repositories;

namespace StudentManagement.Controllers
{
    [Authorize]
    [Authorize(Policy = "RequireAdminRole")]

    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        //Tìm kiếm
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Index(string searchString)
        {
            var departments = _departmentRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments.Where(d => d.DepartmentID.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                               d.DepartmentName.Contains(searchString, StringComparison.OrdinalIgnoreCase));

            }

            return View(departments);
        }

        public IActionResult Details(string id)
        {
            var department = _departmentRepository.GetById(id);
            if (department == null) return NotFound();
            return View(department);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _departmentRepository.Add(department);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Edit(string id)
        {
            var department = _departmentRepository.GetById(id);
            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Department department)
        {
            if (id != department.DepartmentID) return BadRequest();
            if (ModelState.IsValid)
            {
                _departmentRepository.Update(department);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Delete(string id)
        {
            var department = _departmentRepository.GetById(id);
            if (department == null) return NotFound();
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _departmentRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
