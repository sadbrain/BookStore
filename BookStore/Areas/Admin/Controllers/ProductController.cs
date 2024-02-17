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
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

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
        if (id != null && id != 0) ProductVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages");
        if (ProductVM.Product == null) return NotFound();
        return View(ProductVM);
    }
    [HttpPost]
    public IActionResult Upsert(List<IFormFile>? files)
    {
        if (ModelState.IsValid)
        {
            if (ProductVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(ProductVM.Product);
            }
            else
            {
                _unitOfWork.Product.Update(ProductVM.Product);
            }
            _unitOfWork.Save();

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if(files != null)
            {
                foreach(IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images\products\product-" + ProductVM.Product.Id;
                    string filePath = Path.Combine(wwwRootPath, productPath);

                    if(!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = ProductVM.Product.Id
                    };

                    if(ProductVM.Product.ProductImages == null)
                    {
                        ProductVM.Product.ProductImages = new List<ProductImage>();
                    }
                    ProductVM.Product.ProductImages.Add(productImage);
                }
                _unitOfWork.Product.Update(ProductVM.Product);
                _unitOfWork.Save();
            }
            TempData["success"] = "Product created/updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        ProductVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString(),
        });
        return View(ProductVM.Product);
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        Product = _unitOfWork.Product.Get(u => u.Id == id);
        if (Product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        _unitOfWork.Product.Remove(Product);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Product deleted successfully" });
    }
    [HttpGet]
    public IActionResult GetAll()
    {
        var objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return Json(new { data = objProductList });
    }
}
