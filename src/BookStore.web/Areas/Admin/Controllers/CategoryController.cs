using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace BookStore.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_User_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList);
        }


        public ActionResult Details()
        {
            IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
            return View(categoryList);
        }

        //GET
        public ActionResult Create()
        {

            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayOrder is same as the Name");
            }

            if (!ModelState.IsValid)
                return View(category);

            _unitOfWork.Category.Create(category);
            _unitOfWork.Save();

            TempData["success"] = "Category Created Successfylly";

            return RedirectToAction("Index");

        }


        public ActionResult Edit(int id)
        {
            if (id == 0) { return NotFound(); }
            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

            if (category == null) { return NotFound(); }


            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayOrder is same as the Name");
            }

            if (!ModelState.IsValid)
                return View(category);

            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Updated Successfylly";

            return RedirectToAction("Index");
        }



        public ActionResult Delete(int? id)
        {
            if (id == 0) { return NotFound(); }

            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null) { return NotFound(); }


            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult DeletePost(int? id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null) { return NotFound(); }


            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfylly";

            return RedirectToAction("Index");
        }

    }
}
