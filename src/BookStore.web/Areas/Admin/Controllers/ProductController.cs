using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Domain.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;

namespace BookStore.web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _hostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_hostEnvironment = hostEnvironment;
		}

		public ActionResult Index()
		{
			IEnumerable<CoverType> coverTypeList = _unitOfWork.CoverType.GetAll();
			return View(coverTypeList);
		}

		public ActionResult Upsert(int? id)
		{
			ProductViewModel viewModel = new()
			{
				Product = new(),
				CategoryList = _unitOfWork.Category.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
				u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				})
			};

			if (id == null || id == 0)
			{
				return View(viewModel);
			}
			else
			{
				viewModel.Product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == id);
				return View(viewModel);
			}

		}


		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public ActionResult Upsert(ProductViewModel productViewModel, IFormFile? file)
		{
			if (!ModelState.IsValid)
				return View(productViewModel);

			var rootPath = _hostEnvironment.WebRootPath;

			if (file != null)
			{
				string fileName = Guid.NewGuid().ToString();
				var upload = Path.Combine(rootPath, @"images\products");
				var extension = Path.GetExtension(file.FileName);

				if (productViewModel.Product.ImageUrl != null)
				{
					var oldImagePath = Path.Combine(rootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				using (var fileStrams = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
				{
					file.CopyTo(fileStrams);
				}
				productViewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
			}

			if (productViewModel.Product.Id == 0)
			{
				_unitOfWork.Products.Create(productViewModel.Product);
			}
			else
			{
				_unitOfWork.Products.Update(productViewModel.Product);
			}

			_unitOfWork.Save();

			TempData["success"] = "Product created Successfylly";

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
			var productList = _unitOfWork.Products.GetAll(includeProperties: "Category,CoverType");
			return Json(new { data = productList });
		}


		[HttpDelete]
		public ActionResult Delete(int? id)
		{
			var productToDelete = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == id);
			if (productToDelete == null)
			{
				return Json(new { success = false, message = "Error While Deleting." });
			}

			var imagePath = Path.Combine(_hostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(imagePath))
			{
				System.IO.File.Delete(imagePath);
			}

			_unitOfWork.Products.Delete(productToDelete);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Product deleted successfully." });
		}
		#endregion
	}



}
