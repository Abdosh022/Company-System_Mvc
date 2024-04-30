using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    // Inheritance : DepartmentController is  a Controller
    // Aggregation : DepartmentController has a DepartmentRepository [Composition]
    public class DepartmentController : Controller
    {
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        //public DepartmentRepository DepartmentRepository { get; }

        public DepartmentController(IUnitOfWork unitOfWork) // Ask CLR for Creating Object from DepartmentRepository
        {
            //_departmentRepository = /*new DepartmentRepository();*/
            //DepartmentRepository = departmentRepository;

            //_departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        // /Department/Index
        public IActionResult Index()
        {
            var departments = _unitOfWork.DepartmentRepository.GetAll();

            return View(departments);
        }

        // /Department/Create
        //[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if(ModelState.IsValid) // Server-Side Validation [Back-End Validation]
            {
                await _unitOfWork.DepartmentRepository.Add(department);

                int count = await _unitOfWork.Complete();

                // 3. TempData

                if (count > 0)
                    TempData["Message"] = "Department is Created Successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }


        // /Department/Details/1
        // /Department/Details
        //[HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest(); // 400

            var department = await _unitOfWork.DepartmentRepository.Get(id.Value);
            if (department is null)
                return NotFound(); // 404

            return View(viewName, department);
        }


        // /Department/Edit/1
        // /Department/Edit
        //[HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id, "Edit");
            ///if (id is null)
            ///    return BadRequest();
            ///var department = _departmentRepository.Get(id.Value);
            ///if(department is null)
            ///    return NotFound();
            ///return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, Department department)
        {
            if(id != department.Id)
                return BadRequest();


            if(ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    
                    await _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // 1. Log Exception
                    // 2. Show Friendly Message => /Home/Error

                    ModelState.AddModelError("", ex.Message);
                    //return View(department);
                }
                
            }
            return View(department);
        }


        // /Department/Delete/1
        // /Department/Delete
        //[HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
            ///if (id is null)
            ///    return BadRequest();
            ///var department = _departmentRepository.Get(id.Value);
            ///if(department is null)
            ///    return NotFound();
            ///return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute]int id, Department department)
        {
            if (id != department.Id)
                return BadRequest();
            try
            {
                _unitOfWork.DepartmentRepository.Delete(department);

                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                // 2. Show Friendly Message => /Home/Error

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(department);
            }
        }
    }
}
