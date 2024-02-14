using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Diagnostics;
using System.Net.WebSockets;
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
            OrderHeader = new()
        };
        foreach(var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPrieceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
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
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == CartId, tracked: true);
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();
        HttpContext.Session.SetInt32(SD.SessionCart,
    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count());
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Summary()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };
        foreach (var cart in ShoppingCartVM.ShoppingCartList)
        {
            cart.Price = GetPrieceBasedOnQuantity(cart);
            ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }
        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.StrictAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StrictAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
        return View(ShoppingCartVM);
    }
	[HttpPost]
	[ActionName("Summary")]
	public IActionResult SummaryPost()
	{
		var claimsIdentity = User.Identity as ClaimsIdentity;
		var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
		ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
		ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
		foreach (var cart in ShoppingCartVM.ShoppingCartList)
		{
			cart.Price = GetPrieceBasedOnQuantity(cart);
			ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
		}
		ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

		if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            //customer
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        }
        else
        {
			//company
			ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
		}

        _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        //create order detail
        foreach(var cart in ShoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Price,
            };

			_unitOfWork.OrderDetail.Add(orderDetail);
			_unitOfWork.Save();
		}

		//payment
		if (applicationUser.CompanyId.GetValueOrDefault() == 0)
		{
            //customer
			var domain = "https://localhost:7170/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + "customer/cart/index",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in ShoppingCartVM.ShoppingCartList)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}
		return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });

	}
    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties:"ApplicationUser");
        if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
			//place an order by customer
			var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            if(session.PaymentStatus.ToLower() == "paid")
            {
                //thanh toan thanh cong
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }
		}

		//xóa tất cả sản phẩm ở shopping cart
		List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
		_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
		_unitOfWork.Save();

		return View(id);
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
