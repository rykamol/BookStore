using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace BookStore.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            IEnumerable<CoverType> coverTypeList = _unitOfWork.CoverType.GetAll();
            return View(coverTypeList);
        }


        //public ActionResult Details()
        //{
        //    IEnumerable<>> categoryList = _unitOfWork.Category.GetAll();
        //    return View(categoryList);
        //}

        //GET
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Create(CoverType coverType)
        {
            if (coverType.Name == null)
            {
                ModelState.AddModelError("name", "Cover Type is empty");
            }

            if (!ModelState.IsValid)
                return View(coverType);

            _unitOfWork.CoverType.Create(coverType);
            _unitOfWork.Save();

            TempData["success"] = "Cover Type Created Successfylly";

            return RedirectToAction("Index");

        }


        public ActionResult Edit(int id)
        {
            if (id == 0) { return NotFound(); }
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null) { return NotFound(); }


            return View(coverType);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Edit(CoverType coverType)
        {
			if (coverType.Name == null)
			{
				ModelState.AddModelError("name", "Cover Type is empty");
			}

			if (!ModelState.IsValid)
                return View(coverType);

            _unitOfWork.CoverType.Update(coverType);
            _unitOfWork.Save();

            TempData["success"] = "Cover Type Updated Successfylly";

            return RedirectToAction("Index");
        }



        public ActionResult Delete(int? id)
        {
            if (id == 0) { return NotFound(); }

            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null) { return NotFound(); }


            return View(coverType);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult DeletePost(int? id)
        {
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null) { return NotFound(); }


            _unitOfWork.CoverType.Delete(coverType);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type Deleted Successfylly";

            return RedirectToAction("Index");
        }

    }
}
