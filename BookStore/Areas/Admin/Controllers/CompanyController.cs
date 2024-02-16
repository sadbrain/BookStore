using BookStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using BookStore.Models;
using BookStore.Models.ViewModels;
using System;
using BookStore.Utility;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_User_Admin)]
public class CompanyController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [BindProperty]
    public Company Company { get; set; }
    public IActionResult Index()
    {
        var objCompanyList = _unitOfWork.Company.GetAll().ToList();
        return View(objCompanyList);
    }
    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0) Company = new Company(); 
        else Company = _unitOfWork.Company.Get(u => u.Id == id);
        if (Company == null) return NotFound();
        return View(Company);
    }
    [HttpPost]
    public IActionResult Upsert(IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            if (Company.Id == 0)
            {
                _unitOfWork.Company.Add(Company);
                TempData["success"] = "Company created successfully!";
            }
            else
            {
                _unitOfWork.Company.Update(Company);
                TempData["success"] = "Company updated successfully!";
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        return View(Company);
    }
    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
        if (CompanyToBeDeleted == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Company.Remove(CompanyToBeDeleted);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Delete Successful" });
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var objCompanyList = _unitOfWork.Company.GetAll().ToList();
        return Json(new { data = objCompanyList });
    }
}
