using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.web.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public IActionResult Index()
		{
			var productList = _unitOfWork.Products.GetAll("Category,CoverType");
			return View(productList);
		}

		public IActionResult Details(int productId)
		{
			ShoppingCart product = new()
			{
				Count = 1,
				ProductId = productId,
				Product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,CoverType")
			};
			return View(product);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{

			//Retrieve user 
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCart.ApplicationUserId = claim.Value;

			_unitOfWork.ShoppingCarts.Create(shoppingCart);
			_unitOfWork.Save();

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
}