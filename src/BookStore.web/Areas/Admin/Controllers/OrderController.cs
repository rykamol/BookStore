﻿using BookStore.DataAccess.Repository.IRepository;
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
		public IActionResult GetAll()
		{
			var orderHeader = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
			return Json(new { data = orderHeader });
		}
		#endregion
	}
}