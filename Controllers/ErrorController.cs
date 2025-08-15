using Fartak.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IGetAdmin getAdmin;
        public ErrorController(IGetAdmin getAdmin)
        {
            this.getAdmin = getAdmin;
        }

        [Route("Error/Error/{code}")]
        public IActionResult Error(int code)
       {

            Response.StatusCode = code; // حفظ کد در پاسخ نهایی

            switch (code)
            {
                case 401:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("Login","User"); // کاربر احراز هویت نشده
                case 403:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("AccessDenied"); // کاربر مجاز نیست
                case 404:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("NotFound"); // صفحه یا منبع پیدا نشد
                case 500:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("ServerError"); // خطای داخلی سرور
                case 400:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("BadRequest"); // درخواست نامعتبر
                case 408:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("RequestTimeout"); // زمان درخواست تمام شده
                default:
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return View("GenericError"); // سایر خطاها
            }
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult NotFound()
        {
            return View();
        }
        public IActionResult ServerError()
        {
            return View();
        }
        public IActionResult BadRequest()
        {
            return View();
        }
        public IActionResult RequestTimeout()
        {
            return View();
        }
        public IActionResult GenericError()
        {
            return View();
        }
        
    }
}
