using Fartak.DbModels;
using Fartak.Models.Category;
using Fartak.Services;
using Fartak.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataBase context;
        private readonly IGetAdmin getAdmin;


        public CategoryController(DataBase context, IGetAdmin getAdmin)
        {
            this.context = context;
            this.getAdmin = getAdmin;
        }

        public IActionResult AllCategories()
        {
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            var all = context.Categories.ToList();
            return View(all);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            if (getAdmin.IsAdmin())
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View();
            }
            else {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden            }
            }
         }
        [Authorize]
        [HttpPost]
        public IActionResult CreateCategory(Categories cat)
        {
            if (getAdmin.IsAdmin())
            {
                if (cat != null)
                {
                    context.Categories.Add(cat);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllCategories");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllCategories");

            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden            }



        }
        [Authorize]
        [HttpGet]
        public IActionResult EditCategory(int Id)
        {
            
            if (getAdmin.IsAdmin())
            {
                var cate = context.Categories.Find(Id);
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(cate);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult EditCategory(Categories cate)
        {
            if (getAdmin.IsAdmin())
            {
                var cat = context.Categories.Find(cate.Id);
                if (cat != null)
                {
                    cat.Category = cate.Category;
                    cat.Name = cate.Name;
                    cat.Model = cate.Model;
                    context.Categories.Update(cat);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllCategories");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllCategories");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult DeleteCategory(int Id)
        {
            if (getAdmin.IsAdmin())
            {
                var cate = context.Categories.Find(Id);
                if (cate != null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View(cate);
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllCategories");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        

        }
        [Authorize]
        [HttpPost]
        public IActionResult DeleteFromCategory(int Id)
        {
            if (getAdmin.IsAdmin())
            {
                var cate = context.Categories.Find(Id);
                if (cate != null)
                {
                    context.Categories.Remove(cate);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("AllCategories");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllCategories");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden            }

        }


        public IActionResult GetCategories()
        {
            var categories = context.Categories
                .Select(c => c.Category) // انتخاب آیدی و نام دسته‌ها
                .Distinct() // حذف موارد تکراری
                .ToList(); // تبدیل به لیست




            var categoryName = context.Categories
            .Select(c => new NamesInCategory
            {
                Category = c.Category,
                Name = c.Name // شمارش وسایل با این دسته
            })
            .ToList(); // فقط یک نتیجه برمی‌گرداند


            var categoryModel = context.Categories
                .Select(c => new ModelsInCategory
                {
                    Category = c.Category,
                    Name = c.Name,
                    Model = c.Model// شمارش وسایل با این دسته
                })
                .ToList(); // فقط یک نتیجه برمی‌گرداند


            GetAllCategory category = new GetAllCategory()
            {
                Categories = categories,
                NamesCategory = categoryName,
                ModelsCategory = categoryModel
            };
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(category); // ارسال اطل
        }


        //مدیریت دسته بندی ها 
        [HttpGet]
        public IActionResult SearchCategoryNameInProducts(string category)
        {

                var products = context.Product.Where(i => i.Category == category).ToList();
                if (products != null)
                {
                    return View(products);
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("AllCategories");
            

        }

        [HttpGet]
        public IActionResult SearchByNameInProducts(string Name)
        {

            var products = context.Product.Where(i => i.Name == Name).ToList();
            if (products != null)
            {
                return View(products);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("AllCategories");


        }

        public IActionResult SearchByModelInProducts(string model)
        {

            var products = context.Product.Where(i => i.Model == model).ToList();
            if (products != null)
            {
                return View(products);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("AllCategories");


        }

    }
}
