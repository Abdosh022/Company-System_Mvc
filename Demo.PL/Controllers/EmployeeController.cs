using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    public class EmployeeController : Controller
    {
        //private readonly IEmployeeRepository _employeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            //_employeeRepository = employeeRepository;
            //_departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // /Employee/Index
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> Emps;

            if (string.IsNullOrEmpty(SearchValue))
                Emps = await _unitOfWork.EmployeeRepository.GetAll();

            else
                Emps = _unitOfWork.EmployeeRepository.SearchEmployeeByName(SearchValue);

            var mappedEmps = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(Emps);
            return View(mappedEmps);

        }
        // /Employee/Create
        //[HttpGet]
        public IActionResult Create()
        {
            //ViewBag.Departments = _departmentRepository.GetAll();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                // Manual Mapping
                ///var mappedEmp = new Employee()
                ///{
                ///    Name = employeeVM.Name,
                ///    Address = employeeVM.Address,
                ///    Age = employeeVM.Age,
                ///    DepartmentId = employeeVM.DepartmentId,
                ///    Email = employeeVM.Email,
                ///    IsActive = employeeVM.IsActive,
                ///    PhoneNumber = employeeVM.PhoneNumber,
                ///    HireDate = employeeVM.HireDate,
                ///    Salary = employeeVM.Salary
                ///};

                employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "images");

                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                await _unitOfWork.EmployeeRepository.Add(mappedEmp);
                // Update
                // Delete

                await _unitOfWork.Complete();


                return RedirectToAction(nameof(Index));
            }
            return View(employeeVM);
        }


        // /Employee/Details/1
        // /Employee/Details
        //[HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var Employee = await _unitOfWork.EmployeeRepository.Get(id.Value);


            if (Employee is null)
                return NotFound();

            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(Employee);

            return View(viewName, mappedEmp);
        }


        // /Employee/Edit/1
        // /Employee/Edit
        //[HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id, "Edit");
            ///if(id is null)
            ///    return BadRequest();
            ///var Employee = _EmployeeRepository.Get(id.Value);
            ///if (Employee is null)
            ///    return NotFound();
            ///return View(Employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                     _unitOfWork.EmployeeRepository.Update(mappedEmp);
                    await _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // 1. Log Exception
                    // 2. Friendly Message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(employeeVM);
        }


        // /Employee/Delete/1
        // /Employee/Delete
        //[HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();
            try
            {
                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                _unitOfWork.EmployeeRepository.Delete(mappedEmp);
                int count = await _unitOfWork.Complete();
                if (count > 0 && employeeVM.ImageName is not null)
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "images");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                // 2. Friendly Message

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVM);
            }
        }
    }
}
