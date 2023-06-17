using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}


		//[AllowAnonymous]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);


			ShoppingCartViewModel = new ShoppingCartViewModel
			{
				ShoppingCarts = _unitOfWork.ShoppingCarts.GetAll(
					u => u.ApplicationUserId == claim.Value,
					includeProperties: "Product"),
				OrderHeader = new()
			};

			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				item.ProductPrice = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (item.Count * item.ProductPrice);
			}

			return View(ShoppingCartViewModel);
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);


			ShoppingCartViewModel = new ShoppingCartViewModel
			{
				ShoppingCarts = _unitOfWork.ShoppingCarts.GetAll(
					u => u.ApplicationUserId == claim.Value,
					includeProperties: "Product"),
				OrderHeader = new()
			};


			ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicatiionUsers.GetFirstOrDefault(
				u => u.Id == claim.Value);

			ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
			ShoppingCartViewModel.OrderHeader.PhoneNumer = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
			ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
			ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;


			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				item.ProductPrice = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (item.Count * item.ProductPrice);
			}

			//return View(ShoppingCartViewModel);
			return View(ShoppingCartViewModel);
		}

		public IActionResult Plus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCarts.GetFirstOrDefault(c => c.Id == cartId);
			_unitOfWork.ShoppingCarts.IncrementCount(cart, 1);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCarts.GetFirstOrDefault(c => c.Id == cartId);
			if (cart.Count <= 1)
			{
				_unitOfWork.ShoppingCarts.Delete(cart);
			}
			else
			{
				_unitOfWork.ShoppingCarts.DecrementCount(cart, 1);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCarts.GetFirstOrDefault(c => c.Id == cartId);
			_unitOfWork.ShoppingCarts.Delete(cart);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		private double GetPriceByQuantity(int quantity, double price, double Price50, double price100)
		{
			if (quantity <= 50)
			{
				return price;
			}
			else
			{
				if (quantity <= 100)
				{
					return Price50;
				}
				return price100;
			}
		}
	}
}
