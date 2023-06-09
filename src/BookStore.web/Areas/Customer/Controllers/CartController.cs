using Microsoft.AspNetCore.Mvc;

namespace BookStore.web.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
