using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Area.Customer.Controllers;
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCart ShoppingCart { get; set; }
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {

        IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "ProductImages");
        return View(productList);
    }

    public IActionResult Detail(int productId)
    {
        ShoppingCart = new()
        {
            Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "ProductImages"),
            Count = 1,
            ProductId = productId
        };
        return View(ShoppingCart);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Detail()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCart.ApplicationUserId = userId;
        ShoppingCart cartInDb = _unitOfWork.ShoppingCart.Get(u => u.ProductId == ShoppingCart.ProductId && u.ApplicationUserId == ShoppingCart.ApplicationUserId);
        if(cartInDb != null)
        {
            cartInDb.Count = ShoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartInDb);
            _unitOfWork.Save();

        }
        else
        {
            cartInDb = ShoppingCart;
            _unitOfWork.ShoppingCart.Add(cartInDb);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
        }
        TempData["success"] = "Cart updated successfully";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
