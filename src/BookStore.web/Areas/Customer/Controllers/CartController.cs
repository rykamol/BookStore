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
					includeProperties: "Product")
			};

			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				item.ProductPrice = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
				ShoppingCartViewModel.TotalCartPrice += (item.Count * item.ProductPrice);
			}

			return View(ShoppingCartViewModel);
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
