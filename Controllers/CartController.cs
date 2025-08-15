using Fartak.DbModels;
using Fartak.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Controllers
{

    public class CartController : Controller
    {
        private readonly DataBase context;
        private readonly IGetAdmin getAdmin;
        private readonly IMe me;



        public CartController(DataBase context, IGetAdmin getAdmin, IMe me)
        {
            this.context = context;
            this.getAdmin = getAdmin;
            this.me = me;
        }


        [Authorize]
        public IActionResult MyCart() 
        {

            var user = me.UserIsConnected(Request.Cookies["jwt"]);

            if (user == null) {
                Response.Cookies.Delete("jwt");

                // هدایت به صفحه ورود یا خانه
                return RedirectToAction("Login", "user");
            }
            var cart = user.Cart.Split(",").ToList();
            if (cart.Count == 0 || (cart.Count == 1 && string.IsNullOrWhiteSpace(cart[0])))
            {
                ViewData["Message"] = "سبد خرید شما خالی است.";
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View("EmptyCart"); // یا همون View اصلی با پیام
            }
            List<Product> products = new List<Product>();
            foreach (var s in cart)
            {
                foreach (var i in context.Product) {
                    if (i.Id == Convert.ToInt32(s)) {
                        products.Add(i);
                    }
                }

            }

            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(products);
        }

        public IActionResult EmptyCart() 
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [Route("Cart/AddToCart/{Id}")]
        public IActionResult AddToCart(int Id)
        {
            var product = context.Product.Find(Id);
            if (product != null && product.State == true) {
                var user = me.UserIsConnected(Request.Cookies["jwt"]);


                if (user != null)
                {
                    if (user.Cart != "")
                    {
                        var cart = user.Cart.Split(",").ToList();

                        foreach (var i in cart)
                        {
                            if (Convert.ToInt32(i) == product.Id)
                            {
                                ViewBag.IsAdmin = getAdmin.IsAdmin();
                                return View();
                            }

                        }

                        user.Cart = user.Cart +"," +product.Id.ToString() ;
                        context.Users.Update(user);
                        context.SaveChanges();
                        ViewBag.IsAdmin = getAdmin.IsAdmin();
                        return RedirectToAction("mycart", "cart");

                    }
                    else
                    {
                        user.Cart = product.Id.ToString();
                        context.Users.Update(user);
                        context.SaveChanges();
                        ViewBag.IsAdmin = getAdmin.IsAdmin();
                        return RedirectToAction("MyCart", "Cart");

                    }
                }

                return View();

            }
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult RemoveFromCart(int Id)
        {
            var product = context.Product.Find(Id);
            if (product != null)
            {
                var user = me.UserIsConnected(Request.Cookies["jwt"]);


                var cart = user.Cart.Split(",").ToList();
                cart.Remove(Id.ToString());
                user.Cart = string.Join(",", cart);
                context.Users.Update(user);
                context.SaveChanges();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("mycart", "cart");

            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("mycart", "cart");
        }
    }
}
