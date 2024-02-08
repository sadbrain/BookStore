using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Contracts;

namespace BookStore.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    [BindProperty]
    public ProductVM ProductVM { get; set; }
    [BindProperty]
    public Product Product { get; set; }
    public IActionResult Index()
    {
        var objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return View(objProductList);
    }
    public IActionResult Upsert(int? id)
    {
        ProductVM = new() { 
            CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            }),
            Product = new Product()
         };
        if (id != null && id != 0) ProductVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
        if (ProductVM.Product == null) return NotFound();
        return View(ProductVM);
    }
    [HttpPost]
    public IActionResult Upsert()
    {
        if (ModelState.IsValid)
        {
            if (ProductVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(ProductVM.Product);
                TempData["success"] = "Product created successfully!";
            }
            else
            {
                _unitOfWork.Product.Update(ProductVM.Product);
                TempData["success"] = "Product updated successfully!";
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        ProductVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString(),
        });
        return View(ProductVM.Product);
    }
    public IActionResult Delete(int? id)
    {
        if (id == 0 || id == null) return NotFound();
        Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"Category");
        if (Product == null)
        {
            return NotFound();
        }
        return View(Product);
    }
    [HttpPost]
    public IActionResult Delete()
    {
        _unitOfWork.Product.Remove(Product);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
    //[HttpDelete]
    //public IActionResult Delete(int? id)
    //{
    //    Product = _unitOfWork.Product.Get(u => u.Id == id);
    //    if (Product == null)
    //    {
    //        return Json(new { success = false, message = "Error while deleting" });
    //    }
    //    _unitOfWork.Product.Remove(Product);
    //    _unitOfWork.Save();
    //    return Json(new { success = true, message = "Product deleted successfully" });
    //}
    [HttpGet]
    public IActionResult GetAll()
    {
        var objProductList = _unitOfWork.Product.GetAll(includeProperties:"Categort").ToList();
        return Json(new { data = objProductList });
    }
}
