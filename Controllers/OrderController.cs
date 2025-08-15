using Fartak.Interfaces;
using Fartak.DbModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartak.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Fartak.Controllers
{
    public class OrderController : Controller
    {

        private readonly DataBase context;
        private readonly IGetAdmin getAdmin;
        private readonly IMe me;

        public OrderController(DataBase context, IGetAdmin getAdmin, IMe me)
        {
            this.context = context;
            this.getAdmin = getAdmin;
            this.me = me;
        }

        [Authorize]
        [HttpPost]
        [Route("order/addToOrders")]
        public IActionResult AddToOrders([FromBody] List<OrderOdd> order)
        {

            if (!ModelState.IsValid) {
                return Ok();
            }
            string ord = "";
            foreach (var i in order) {
                if (ord == "")
                {
                    ord = i.ProductId + "," + i.ProductName + "," + i.Number + "," + i.Price ;
                }
                else {
                    ord = ord + "/"+ i.ProductId + "," + i.ProductName + "," + i.Number + "," + i.Price ;
                }
            }
            var user = me.UserIsConnected(Request.Cookies["jwt"]);
            if (user != null) {
                Order orderForDb = new Order()
                {
                    Orders = ord,
                    State = "پرداخت نشده",
                    UserId = user.Id,
                    CreateDate = DateTime.Now.ToString()
                };

                context.Orders.Add(orderForDb);
                user.Cart = "";
                context.Users.Update(user);
                context.SaveChanges();
                return RedirectToAction("MyOrders","Order");
            }
            return RedirectToAction("MyCart","Cart");



        }

        [Authorize]
        [HttpGet]
        public IActionResult MyOrders() {

            var user = me.UserIsConnected(Request.Cookies["jwt"]);
            if (user == null)
            {
                Response.Cookies.Delete("jwt");

                // هدایت به صفحه ورود یا خانه
                return RedirectToAction("Login", "user");
            }
           
                
                var orders = context.Orders.Where(i => i.UserId == user.Id).ToList();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(orders);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserOrders(int id)
        {

            var user = context.Users.Find(id);
            if (user == null)
            {
                Response.Cookies.Delete("jwt");

                // هدایت به صفحه ورود یا خانه
                return RedirectToAction("Login", "user");
            }

            List<Order> orders = new List<Order>();
            orders = context.Orders.Where(i => i.UserId == user.Id).ToList();
            OrderModel orderModel = new OrderModel() { orders = orders,user=user };
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return View(orderModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AllOrders()
        {
            if (getAdmin.IsAdmin())
            {
                var user = me.UserIsConnected(Request.Cookies["jwt"]);
                if (user == null)
                {
                    Response.Cookies.Delete("jwt");

                    // هدایت به صفحه ورود یا خانه
                    return RedirectToAction("Login", "user");
                }


                var orders = context.Orders.ToList();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(orders);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUserFromOrder(string id)
        {
            if (getAdmin.IsAdmin())
            {
                var myUser = context.Users.Find(Convert.ToInt32(id));
                if (myUser == null)
                {
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return NotFound(); // اگر کاربر پیدا نشود
                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return View(myUser);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403);
        }


        [Authorize]
        [HttpGet]
        public IActionResult ShowProduct(string Id)
        {

            var product = context.Product.Find(Convert.ToInt32(Id));

            if (product != null)
            {

                return View(product);
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("MyOrders");
        }
        [Authorize]
        [HttpGet]
        public IActionResult DeleteOrder(string Id)
        {

            var order = context.Orders.Find(Convert.ToInt32(Id));

            if (order != null)
            {
                context.Orders.Remove(order);
                context.SaveChanges();
                return RedirectToAction("MyOrders");

            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("MyOrders");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Payment(string Id)
        {
            //کاربر با ارسال مبلغ از درگاه بانکی وضعیت را به در حال پردازش تغییر میدهد تا توسط مدیریت تایید شود برای ارسال
            var order = context.Orders.Find(Convert.ToInt32(Id));

            if (order != null)
            {
                order.State = "درحال پردازش";
                order.LastChangeDate = DateTime.Now.ToString();
                context.Orders.Update(order);
                context.SaveChanges();
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("MyOrders");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return RedirectToAction("MyOrders");
        }

        [Authorize]
        [HttpGet]
        public IActionResult PaymentWithAdmin(string Id)
        {
            if (getAdmin.IsAdmin()) {
                var order = context.Orders.Find(Convert.ToInt32(Id));

                if (order != null)
                {
                    order.State = "درحال پردازش";
                    order.LastChangeDate = DateTime.Now.ToString();
                    context.Orders.Update(order);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("Users","User");

                }
                ViewBag.IsAdmin = getAdmin.IsAdmin();
                return RedirectToAction("Users", "User");
            }
            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpGet]
        public IActionResult ReadyForSend(string Id)
        {
            if (getAdmin.IsAdmin())
            {
                var order = context.Orders.Find(Convert.ToInt32(Id));

                if (order != null)
                {
                    order.State = "آماده برای ارسال";
                    order.LastChangeDate = DateTime.Now.ToString();
                    context.Orders.Update(order);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("Users", "User");
                }
            }

            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpGet]
        public IActionResult SendProducts(string Id)
        {
            if (getAdmin.IsAdmin())
            {
                var order = context.Orders.Find(Convert.ToInt32(Id));

                if (order != null)
                {
                    order.State = "بسته شما به آدرس ثبت شده ارسال شد";
                    order.LastChangeDate = DateTime.Now.ToString();
                    context.Orders.Update(order);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("Users", "User");
                }
            }

            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }

        [Authorize]
        [HttpGet]
        public IActionResult ReceiveProducts(string Id)
        {
            if (getAdmin.IsAdmin())
            {
                var order = context.Orders.Find(Convert.ToInt32(Id));

                if (order != null)
                {
                    order.State = "بسته به دست مشتری رسید";
                    order.LastChangeDate = DateTime.Now.ToString();
                    context.Orders.Update(order);
                    context.SaveChanges();
                    ViewBag.IsAdmin = getAdmin.IsAdmin();
                    return RedirectToAction("Users", "User");
                }
            }

            ViewBag.IsAdmin = getAdmin.IsAdmin();
            return StatusCode(403); // Forbidden        
        }


        ///

    }
}
