using BookStore.DataAccess.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		#region  API CALLS
		public IActionResult GetAll(string status)
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");

			switch (status)
			{
				case "pending":
					orderHeader = orderHeader.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;

				case "inprogress":
					orderHeader = orderHeader.Where(u => u.PaymentStatus == SD.StatusInProgress);
					break;

				case "completed":
					orderHeader = orderHeader.Where(u => u.PaymentStatus == SD.StatusShipped);
					break;
				case "approved":
					orderHeader = orderHeader.Where(u => u.PaymentStatus == SD.StatusApprove);
					break;

				default:
					break;
			}
			return Json(new { data = orderHeader });
		}
		#endregion
	}
}
