using Fartak.DbModels;
using Fartak.Models.Product;
using Fartak.Interfaces;
using Fartak.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Fartak.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataBase context;
        private readonly IWebHostEnvironment Environment;
        private readonly IGetAdmin getAdmin;



        public ProductController(DataBase context, IWebHostEnvironment Environment, IGetAdmin getAdmin)
        {
            this.context = context;
            this.Environment = Environment;
            this.getAdmin = getAdmin;
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            if (getAdmin.IsAdmin()) {
                // لیست دسته‌بندی‌ها
                var categoryList = context.Categories
                    .Select(c => c.Category)
                    .Distinct()
                    .ToList();
                
                ViewBag.Categories = categoryList;
                ViewBag.IsAdmin = getAdmin.IsAdmin();
               
                return View();
            }

            return StatusCode(403); // Forbidden        
        }

        public JsonResult GetNamesByCategory(string category)
        {
            var names = context.Categories
                .Where(c => c.Category == category)
                .Select(c => c.Name)
                .Distinct()
                .ToList();

            return Json(names);
        }
        [HttpGet]
        public JsonResult GetModelsByName(string name)
        {
            var models = context.Categories
                .Where(c => c.Name == name)
                .Select(c => c.Model)
                .Distinct()
                .ToList();

            return Json(models);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateProduct(CreateProduct product)
        {
            if (getAdmin.IsAdmin())
            {
                if (ModelState.IsValid)
                {
                    UploadFile updluadFile = new UploadFile(Environment, context, product.Model);
                    Product prod = new Product()
                    {
                        Category = product.Category,
                        Model = product.Model,
                        Name = product.Name,
                        NameForView = product.NameForView,
                        Price = product.Price.ToString(),
                        Colors = product.Colors,
                        AboutThisProduct = product.AboutThisProduct,
                        PictureNumbers = updluadFile.Uploads(product.Pictures.Files),
                    };
                    context.Product.Add(prod);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllProducts");
                }
                else
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View();
                }
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        

        }

        public IActionResult AllProducts()
        {
            var products = context.Product.ToList();

            var model = new DashboardViewModel
            {
                Product = products,
                IsAdmin = getAdmin
            };
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(model);
        }
        [Authorize]
        public IActionResult RemoveProd(int id)
        {
            if (getAdmin.IsAdmin())
            { 
                var product = context.Product.Find(id);
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(product);
            }else{
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }
        [Authorize]
        public IActionResult RemoveProduct(int Id)
        {
            if (getAdmin.IsAdmin())
            {
                var product = context.Product.Find(Id);
                if (product != null)
                {
                    context.Product.Remove(product);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllProducts");

                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllProducts");

            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditProd(int id)
        {
            if (getAdmin.IsAdmin())
            {
                var product = context.Product.Find(id);

                if (product != null)
                {
                    EditProductsModel newPro = new EditProductsModel()
                    {
                        Id = product.Id,
                        Category = product.Category,
                        Name = product.Name,
                        Model = product.Model,
                        Colors = product.Colors,
                        NameForView = product.NameForView,
                        Price = Convert.ToInt32(product.Price),
                        AboutThisProduct = product.AboutThisProduct,
                        State = product.State,
                    };
                    var categoryList = context.Categories
                    .Select(c => c.Category)
                    .Distinct()
                    .ToList();

                    ViewBag.Categories = categoryList;
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View(newPro);
                }
                else {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AccessDenied", "Error");
                }
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProduct(EditProductsModel product)
        {
            if (getAdmin.IsAdmin())
            {
                if (ModelState.IsValid)
                {
                    var pro = context.Product.Find(product.Id);

                    UploadFile updluadFile = new UploadFile(Environment, context, product.Model);
                    pro.Category = product.Category;
                    pro.Model = product.Model;
                    pro.Name = product.Name;
                    pro.NameForView = product.NameForView;
                    pro.Price = product.Price.ToString();
                    pro.Colors = product.Colors;
                    pro.State = product.State;
                    pro.AboutThisProduct = product.AboutThisProduct;
                    pro.PictureNumbers = product.PictureNumbers;
                    if (product.Pictures.Files.Count != 0)
                    {
                        pro.PictureNumbers =  updluadFile.Uploads(product.Pictures.Files);
                    }
                    context.Product.Update(pro);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllProducts");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllProducts");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpPost]
        [Route("Product/DeletePictures/{Id}")]
        public IActionResult DeletePictures(int Id)
        {
            if (getAdmin.IsAdmin())
            {
                var product = context.Product.Find(Id);
                if (product != null)
                {

                    product.PictureNumbers = "";
                    Console.WriteLine(product.PictureNumbers);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllProducts");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllProducts");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [HttpGet]
        public IActionResult ShowProduct(int Id)
        {

            var product = context.Product.Find(Id);

            if (product != null)
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(product);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("AllProducts");
        }

    }
}
