using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
		public IActionResult Details(int id)
		{
			ShoppingCart product = new()
			{
				Count = 1,
				Product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == id, includeProperties: "Category,CoverType")
			};
			return View(product);
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