using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
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
			IEnumerable<OrderHeader> orderHeader;

			if (User.IsInRole(SD.Role_User_Admin) || User.IsInRole(SD.Role_User_Employee))
			{
				orderHeader = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
			}
			else
			{
				var claimsIdentity = User.Identity as ClaimsIdentity;
				var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
				orderHeader = _unitOfWork.OrderHeaders.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
			}

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
