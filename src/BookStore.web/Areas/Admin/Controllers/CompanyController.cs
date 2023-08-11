using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;

namespace BookStore.web.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles =SD.Role_User_Admin)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public ActionResult Index()
		{
			IEnumerable<CoverType> coverTypeList = _unitOfWork.CoverType.GetAll();
			return View(coverTypeList);
		}

		public ActionResult Upsert(int? id)
		{
			Company company = new();
			 
			if (id == null || id == 0)
			{
				return View(company);
			}
			else
			{
				company = _unitOfWork.Companies.GetFirstOrDefault(x => x.Id == id);
				return View(company);
			}

		}


		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public ActionResult Upsert(Company company)
		{
			if (!ModelState.IsValid)
				return View(company);

			 
			if (company.Id == 0)
			{
				_unitOfWork.Companies.Create(company);
				TempData["success"] = "Company Updated Successfylly";
			}
			else
			{
				_unitOfWork.Companies.Update(company);
				TempData["success"] = "Company Updated Successfylly";
			}

			_unitOfWork.Save();
			return RedirectToAction("Index");
		}



		//public ActionResult Delete(int? id)
		//{
		//	if (id == 0) { return NotFound(); }

		//	var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
		//	if (coverType == null) { return NotFound(); }


		//	return View(coverType);
		//}

		//[HttpPost]
		//[AutoValidateAntiforgeryToken]
		//public ActionResult DeletePost(int? id)
		//{
		//	var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
		//	if (coverType == null) { return NotFound(); }


		//	_unitOfWork.CoverType.Delete(coverType);
		//	_unitOfWork.Save();
		//	TempData["success"] = "Cover Type Deleted Successfylly";

		//	return RedirectToAction("Index");
		//}

		#region  API CALLS
		public IActionResult GetAll()
		{
			var companyList = _unitOfWork.Companies.GetAll();
			return Json(new { data = companyList });
		}

		[HttpDelete]
		public ActionResult Delete(int? id)
		{
			var companyToDelete = _unitOfWork.Companies.GetFirstOrDefault(x => x.Id == id);
			if (companyToDelete == null)
			{
				return Json(new { success = false, message = "Error While Deleting." });
			}

			_unitOfWork.Companies.Delete(companyToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Product deleted successfully." });
		}
		#endregion
	}



}
