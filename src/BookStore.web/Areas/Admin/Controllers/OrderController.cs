using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace BookStore.web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderViewModel orderViewModel { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			orderViewModel = new OrderViewModel()
			{
				OrderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetails.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return View(orderViewModel);
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderViewModel.OrderHeader.Id);
			orderHeader.Name = orderViewModel.OrderHeader.Name;
			orderHeader.PhoneNumer = orderViewModel.OrderHeader.PhoneNumer;
			orderHeader.State = orderViewModel.OrderHeader.State;
			orderHeader.StreetAddress = orderViewModel.OrderHeader.StreetAddress;
			orderHeader.City = orderViewModel.OrderHeader.City;
			orderHeader.PostalCode = orderViewModel.OrderHeader.PostalCode;

			if (orderViewModel.OrderHeader.Carrier != null)
			{
				orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
			}

			if (orderViewModel.OrderHeader.TrackingNumer != null)
			{
				orderHeader.Carrier = orderViewModel.OrderHeader.TrackingNumer;
			}


			_unitOfWork.OrderHeaders.Update(orderHeader);
			_unitOfWork.Save();

			TempData["Success"] = $"Order Details Updated Successfully";

			return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
		}



		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderViewModel.OrderHeader.Id);
			_unitOfWork.OrderHeaders.UpdateOrderStatus(orderViewModel.OrderHeader.Id, SD.StatusInProgress);
			_unitOfWork.Save();

			TempData["Success"] = $"Order Status Updated Successfully";

			return RedirectToAction("Details", "Order", new { orderId = orderViewModel.OrderHeader.Id });
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderViewModel.OrderHeader.Id);

			orderHeader.TrackingNumer = orderViewModel.OrderHeader.TrackingNumer;
			orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}

			_unitOfWork.OrderHeaders.Update(orderHeader);
			_unitOfWork.Save();

			TempData["Success"] = $"Order Shipped Successfully";

			return RedirectToAction("Details", "Order", new { orderId = orderViewModel.OrderHeader.Id });
		}



		[HttpPost]
		[Authorize(Roles = SD.Role_User_Admin + "," + SD.Role_User_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(u => u.Id == orderViewModel.OrderHeader.Id);

			if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,
				}; 
				
				var service = new RefundService();
				var refund = service.Create(options);

				_unitOfWork.OrderHeaders.UpdateOrderStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeaders.UpdateOrderStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);

			}
			_unitOfWork.Save();
			TempData["Success"] = $"Order Canceled Successfully";
			return RedirectToAction("Details", "Order", new { orderId = orderViewModel.OrderHeader.Id });
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
