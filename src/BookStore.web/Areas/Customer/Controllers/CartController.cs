using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BookStore.web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);


			ShoppingCartViewModel.ShoppingCarts = _unitOfWork.ShoppingCarts.GetAll(
				u => u.ApplicationUserId == claim.Value,
				includeProperties: "Product");



			ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				item.ProductPrice = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (item.Count * item.ProductPrice);
			}

			var applicationUser = _unitOfWork.ApplicatiionUsers.GetFirstOrDefault(u => u.Id == claim.Value);
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending.ToString();
				ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending.ToString();
			}
			else
			{

				ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment.ToString();
				ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusApprove.ToString();
			}

			_unitOfWork.OrderHeaders.Create(ShoppingCartViewModel.OrderHeader);
			_unitOfWork.Save();

			foreach (var item in ShoppingCartViewModel.ShoppingCarts)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = item.ProductId,
					OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
					Price = item.ProductPrice,
					Count = item.Count
				};
				_unitOfWork.OrderDetails.Create(orderDetail);
				_unitOfWork.Save();
			}


			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{


				var domain = "https://localhost:44339/";
				var options = new SessionCreateOptions
				{
					PaymentMethodTypes = new List<string>
				{
					"card"
				},
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
					CancelUrl = domain + $"customer/cart/Index",
				};

				foreach (var item in ShoppingCartViewModel.ShoppingCarts)
				{

					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(double)item.ProductPrice * 100,
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product?.Title
							},
						},
						Quantity = item.Count
					};

					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);
				_unitOfWork.OrderHeaders.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id);
				_unitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			else
			{
				return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartViewModel.OrderHeader.Id });
			}

			//_unitOfWork.ShoppingCarts.RemoveRange(ShoppingCartViewModel.ShoppingCarts);
			//_unitOfWork.Save();

			//return RedirectToAction("Index");
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

		public IActionResult OrderConfirmation(int id)
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(x => x.Id == id);

			if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{

				var service = new SessionService();
				var session = service.Get(orderHeader.SeassionId);

				var paymentIntentId = session.PaymentIntentId;

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeaders.UpdateOrderStatus(id, SD.StatusApprove, SD.PaymentStatusApproved, paymentIntentId);
					_unitOfWork.Save();
				}
			}

		
			var shoppingCarts = _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCarts.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);

			//checked the stripe status
		}
	}
}

