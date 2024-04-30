using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AutoMapper;

namespace Demo.PL.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;

		public UserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
			_userManager = userManager;
			_mapper = mapper;
		}

		// /User/Index
		public async Task<IActionResult> Index(string SearchValue)
		{
			List<ApplicationUser> users = new List<ApplicationUser>();

			if (string.IsNullOrEmpty(SearchValue))
				users.AddRange(_userManager.Users.ToList());

			else
			{
				var user = await _userManager.FindByNameAsync(SearchValue); // UserName
				users.Add(user);

			}

			var mappedUsers = _mapper.Map<List<ApplicationUser>, List<UserViewModel>>(users);
			return View(mappedUsers);


		}


		//// /Employee/Create
		////[HttpGet]
		//public IActionResult Create()
		//{
		//	//ViewBag.Departments = _departmentRepository.GetAll();

		//	return View();
		//}

		//[HttpPost]
		//public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
		//{
		//	if (ModelState.IsValid) // Server Side Validation
		//	{
		//		// Manual Mapping
		//		///var mappedEmp = new Employee()
		//		///{
		//		///    Name = employeeVM.Name,
		//		///    Address = employeeVM.Address,
		//		///    Age = employeeVM.Age,
		//		///    DepartmentId = employeeVM.DepartmentId,
		//		///    Email = employeeVM.Email,
		//		///    IsActive = employeeVM.IsActive,
		//		///    PhoneNumber = employeeVM.PhoneNumber,
		//		///    HireDate = employeeVM.HireDate,
		//		///    Salary = employeeVM.Salary
		//		///};

		//		employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "images");

		//		var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

		//		await _unitOfWork.EmployeeRepository.Add(mappedEmp);
		//		// Update
		//		// Delete

		//		await _unitOfWork.Complete();


		//		return RedirectToAction(nameof(Index));
		//	}
		//	return View(employeeVM);
		//}


		// /Employee/Details/1
		// /Employee/Details
		//[HttpGet]

		public async Task<IActionResult> Details(string id, string viewName = "Details")
		{
			if (id is null)
				return BadRequest();
			var user = await _userManager.FindByIdAsync(id);


			if (user is null)
				return NotFound();

			var mappedEmp = _mapper.Map<ApplicationUser, UserViewModel>(user);

			return View(viewName, mappedEmp);
		}


        // /User/Edit/1
        // /User/Edit
        //[HttpGet]
        public async Task<IActionResult> Edit(string id)
		{
			return await Details(id, "Edit");
			
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromRoute] string id, UserViewModel updatedUser)
		{
			if (id != updatedUser.Id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				try
				{
					var user = await _userManager.FindByIdAsync(id);

					user.FName = updatedUser.FName;
					user.LName = updatedUser.LName;
					user.PhoneNumber = updatedUser.PhoneNumber;
					//user.Email = updatedUser.Email;
					


					await _userManager.UpdateAsync(user);

					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					// 1. Log Exception
					// 2. Friendly Message

					ModelState.AddModelError(string.Empty, ex.Message);
				}

			}
			return View(updatedUser);
		}


		// /Employee/Delete/1
		// /Employee/Delete
		//[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			return await Details(id, "Delete");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete([FromRoute] string id, UserViewModel deletedUser)
		{
			if (id != deletedUser.Id)
				return BadRequest();
			try
			{
				var user = await _userManager.FindByIdAsync(id);
				await _userManager.DeleteAsync(user);

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				// 1. Log Exception
				// 2. Friendly Message

				ModelState.AddModelError(string.Empty, ex.Message);
				return View(deletedUser);
			}
		}
	}
}
