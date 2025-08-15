using Fartak.DbModels;
using Fartak.Models;
using Fartak.Services;
using Fartak.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fartak.Controllers
{
    //[Authorize]
    //[ApiController]
    //[Route("[controller]")]
    public class UserController : Controller
    {

        private readonly DataBase context;
        private readonly IGetAdmin getAdmin;
        private readonly IMe me;

        public UserController(DataBase context, IGetAdmin getAdmin, IMe me)
        {
            this.context = context;
            this.getAdmin = getAdmin;
            this.me = me;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("jwt");

            // هدایت به صفحه ورود یا خانه
            return RedirectToAction("Login", "user");
        }

        [Authorize]
        public IActionResult Users()
        {
            var user = me.UserIsConnected(Request.Cookies["jwt"]);

            if (user == null)
            {
                Response.Cookies.Delete("jwt");

                // هدایت به صفحه ورود یا خانه
                return RedirectToAction("Login", "user");
            }
            if (getAdmin.IsAdmin()) {
                List<User> users = context.Users.ToList();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(users);

            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("Allproducts","product");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register model)
        {
            if (model != null)
            {
                // ایجاد کاربر جدید
                var user = new User
                {
                    Name = model.Name,
                    Family   = model.Family,
                    MelliCode = model.MelliCode,
                    Password = model.Password,
                    Address = model.Address, // یا هر فیلد دیگری که می‌خواهید استفاده کنید
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Cart = ""// اگر شماره تلفن را ذخیره می‌کنید
                };

                context.Users.Add(user);
                context.SaveChanges();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("Users", "User");
                // ثبت‌نام کاربر
               
                // اگر ثبت‌نام با خطا مواجه شد، پیام‌های خطا را به مدل اضافه کنید
               
            }

            ViewBag.IsAdmin = getAdmin.IsAdmin();
            // در صورت وجود خطا، فرم را با مدل جاری دوباره نشان دهید
            return RedirectToAction("Register", "User");
        }


        [Authorize]
        [HttpGet]
        public IActionResult Remove(int id)
        {
            if (getAdmin.IsAdmin())
            {

                var user = context.Users.Find(id);

                if (user == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("این کاربر را نمیتوان پیدا  کرد");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(user);
            }
            else
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveUser(int id)
        {
            if (getAdmin.IsAdmin())
            {
                var user = context.Users.Find(id);
                if (user == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return NotFound(); // اگر کاربر پیدا نشود
                }

                context.Users.Remove(user); // حذف کاربر
                context.SaveChanges(); // ذخیره تغییرات در پایگاه داده
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("Users"); // هدایت به لیست کاربران
            }
            else {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (getAdmin.IsAdmin())
            {
                var user = context.Users.Find(id);

                if (user == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("این کاربر را نمیتوان پیدا  کرد");
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(user);
            }
            else {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (getAdmin.IsAdmin())
            {
                var myUser = context.Users.Find(user.Id);
                if (myUser == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return NotFound(); // اگر کاربر پیدا نشود
                }

                myUser.Family = user.Family;
                myUser.Email = user.Email;
                myUser.Address = user.Address;
                myUser.Name = user.Name;
                myUser.MelliCode = user.MelliCode;
                myUser.PhoneNumber = user.PhoneNumber;


                context.Users.Update(myUser); // حذف کاربر
                context.SaveChanges(); // ذخیره تغییرات در پایگاه داده
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("Users"); // هدایت به لیست کاربران
            }
            else
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult MyProfile()
        {
            var user = me.UserIsConnected(Request.Cookies["jwt"]);
            if (user == null)
            {
                Response.Cookies.Delete("jwt");

                // هدایت به صفحه ورود یا خانه
                return RedirectToAction("Login", "user");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DetailUser(User user)
        {
            if (getAdmin.IsAdmin())
            {
                var myUser = context.Users.Find(user.Id);
                if (myUser == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return NotFound(); // اگر کاربر پیدا نشود
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(myUser);
            }
            else
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult UpgradeToAdmin(int id)
        {
            if (getAdmin.IsAdmin())
            {
                var myUser = context.Users.Find(id);
                if (myUser == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return NotFound(); // اگر کاربر پیدا نشود
                }

                myUser.IsAdmin = true;


                context.Users.Update(myUser); // حذف کاربر
                context.SaveChanges(); // ذخیره تغییرات در پایگاه داده
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("Users"); // هدایت به لیست کاربران
            }
            else
            {
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return StatusCode(403); // Forbidden        
            }
        }
    }

    
}
