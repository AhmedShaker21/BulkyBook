using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }


        // ============================
        //       CREATE / UPDATE
        // ============================
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                // Create
                return View(productVM);
            }
            else
            {
                // Update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productVM);
            }
        }


        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                // Insert or Update Base Product
                if (productVM.Product.Id == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);

                _unitOfWork.Save();


                // ======================
                //     IMAGE UPLOAD
                // ======================

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (files != null && files.Count > 0)
                {
                    string productFolder = Path.Combine(wwwRootPath, $"images/products/product-{productVM.Product.Id}");

                    if (!Directory.Exists(productFolder))
                        Directory.CreateDirectory(productFolder);

                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string finalPath = Path.Combine(productFolder, fileName);

                        using (var stream = new FileStream(finalPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Save ImageUrl to DB with forward slashes (browser compatible)
                        ProductImage productImage = new()
                        {
                            ImageUrl = $"/images/products/product-{productVM.Product.Id}/{fileName}",
                            ProductId = productVM.Product.Id
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }


                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }

            // Reload list if failed
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(productVM);
        }


        // ============================
        //       DELETE IMAGE
        // ============================
        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);

            if (imageToBeDeleted == null)
                return NotFound();

            int productId = imageToBeDeleted.ProductId;

            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                    imageToBeDeleted.ImageUrl.TrimStart('/').Replace("/", "\\"));

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }

            _unitOfWork.ProductImage.Remove(imageToBeDeleted);
            _unitOfWork.Save();

            TempData["success"] = "Image deleted successfully";

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        // ============================
        //       API CALLS
        // ============================
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if (productToBeDeleted == null)
                return Json(new { success = false, message = "Error while deleting" });

            string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, $"images/products/product-{id}");

            if (Directory.Exists(folderPath))
            {
                foreach (var file in Directory.GetFiles(folderPath))
                    System.IO.File.Delete(file);

                Directory.Delete(folderPath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
    }
}
