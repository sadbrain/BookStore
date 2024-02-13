using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Contracts;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = SD.Role_User_Admin)]
public class CategoryController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    [BindProperty]
    public Category Category { get; set; }
    public IActionResult Index()
    {
        var objCategoryList = _unitOfWork.Category.GetAll().ToList();
        return View(objCategoryList);
    }
    public IActionResult Upsert(int? id)
    {
        if (id == null || id == 0) Category = new Category();
        else Category = _unitOfWork.Category.Get(u => u.Id == id);
        if (Category == null) return NotFound();
        return View(Category);
    }
    [HttpPost]
    public IActionResult Upsert()
    {
        if (ModelState.IsValid)
        {
            if (Category.Id == 0)
            {
                _unitOfWork.Category.Add(Category);
                TempData["success"] = "Category created successfully!";
            }
            else
            {
                _unitOfWork.Category.Update(Category);
                TempData["success"] = "Category updated successfully!";
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        return View(Category);
    }
    public IActionResult Delete(int? id)
    {
        if (id == 0 || id == null) return NotFound();
        Category = _unitOfWork.Category.Get(u => u.Id == id);
        if (Category == null)
        {
            return NotFound();
        }
        return View(Category);
    }
    [HttpPost]
    public IActionResult Delete()
    {
        _unitOfWork.Category.Remove(Category);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
    //[HttpDelete]
    //public IActionResult Delete(int? id)
    //{
    //    Category = _unitOfWork.Category.Get(u => u.Id == id);
    //    if (Category == null)
    //    {
    //        return Json(new { success = false, message = "Error while deleting" });
    //    }
    //    _unitOfWork.Category.Remove(Category);
    //    _unitOfWork.Save();
    //    return Json(new { success = true, message = "Category deleted successfully" });
    //}
    [HttpGet]
    public IActionResult GetAll()
    {
        var objCategoryList = _unitOfWork.Category.GetAll().ToList();
        return Json(new { data = objCategoryList });
    }
}
