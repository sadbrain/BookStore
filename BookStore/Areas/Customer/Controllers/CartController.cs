using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Area.Customer.Controllers;
[Area("Customer")]
[Authorize]
public class CartController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public IActionResult Index()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
        };
        foreach(var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPrieceBasedOnQuantity(cart);
            ShoppingCartVM.OrderTotal += cart.Price * cart.Count;
        }
        return View(ShoppingCartVM);
    }
    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        if(cartFromDb.Count != 0) cartFromDb.Count -= 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Remove(int CartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId);
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    private double GetPrieceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if(shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else if(shoppingCart.Count <= 100)
        {
            return shoppingCart.Product.Price50;
        }
        else
        {
            return shoppingCart.Product.Price100;
        }
    }
}
