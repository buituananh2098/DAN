using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAN.Models;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Configuration;

namespace DAN.Controllers
{
    public class OrderController : Controller
    {
        DANEntities db = new DANEntities();
        [HttpGet]
        public ActionResult Index()
        {
            var model = new OrderViewModel();
            if (Session["Cart"] == null)
                return RedirectToAction("Index", "Cart");
            if (Session["User"] != null)
            {
                model = (OrderViewModel)Session["User"];
                return View(model);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Code != null)
                {
                    var code = db.Codes.SingleOrDefault(e => e.Code1 == model.Code);
                    if (code == null || code.Expired.Value)
                    {
                        ModelState.AddModelError("Code", "Mã khuyến mãi không hợp lệ hoặc đã được sử dụng!");
                        return View("Index");
                    }
                }
                Session["User"] = model;
                return View(model);
            }

            return View("Index");
        }

        private void setDataVnPay()
        {
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.0.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

            string locale = "vn";
            if (!string.IsNullOrEmpty(locale))
            {
                vnpay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }

            vnpay.AddRequestData("vnp_CurrCode", "VND");
            //vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());
            //vnpay.AddRequestData("vnp_OrderInfo", order.OrderDescription);
            //vnpay.AddRequestData("vnp_OrderType", orderCategory.SelectedItem.Value); //default value: other
            //vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            //vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            //vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            //vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));

            //if (bank.SelectedItem != null && !string.IsNullOrEmpty(bank.SelectedItem.Value))
            //{
            //    vnpay.AddRequestData("vnp_BankCode", bank.SelectedItem.Value);
            //}

            //string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            //Response.Redirect(paymentUrl);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Done()
        {
            var user = (OrderViewModel)Session["User"];
            var cart = (List<Product>)Session["Cart"];
            setDataVnPay();
            try
            {
                var order = new Order()
                {
                    Prefix = "DAN-",
                    Fullname = user.fullName,
                    Address = user.adress,
                    Phone = user.phone.ToString(),
                    Email = user.email,
                    Code = user.Code ?? "Không sử dụng",
                    CreatOn = DateTime.Now,
                    TotalPrice = cart.Sum(e => e.Quantum * e.SalePrice),
                    PaidInfo = user.PaidInfo
                };
                db.Orders.Add(order);
                db.SaveChanges();
                foreach (var item in cart)
                {
                    db.OrderDetails.Add(new OrderDetail()
                    {
                        Pname = item.Pname,
                        OId = order.Id,
                        Quantum = item.Quantum,
                        Price = item.Quantum * item.SalePrice
                    });
                    db.SaveChanges();
                }
                Session.Remove("Cart");
                return View("Done");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra, Vui lòng thử lại!");
                return View("Index");
            }
        }
        public ActionResult SearchInvoice()
        {
            return View("SearchInvoice");
        }
        [HttpPost]
        public ActionResult SearchInvoice(SearchInvoiceModel model)
        {
            if (ModelState.IsValid)
            {
                var order = db.Orders.SingleOrDefault(e => e.OId == model.OId);
                if (order == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy đơn hàng nào!");
                    return View(model);
                }
                else
                    return RedirectToAction("Invoice", new { OId = model.OId });
            }
            return View(model);
        }
  
        public ActionResult Invoice(string OId)
        {
            var order = db.Orders.FirstOrDefault(e => e.OId == OId);
            var orderDetail = db.OrderDetails.Where(e => e.OId == order.Id);
            if (order == null)
                return RedirectToAction("SearchInvoice");
           
            var model = new InvoiceViewModel()
            {
                order = order,
                orderDetail = orderDetail
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Invoice(string key, string OId, string returnUrl)
        {
            switch (key)
            {
                case "status":
                    var order = db.Orders.SingleOrDefault(e => e.OId == OId);
                    order.Status = true;
                    db.SaveChanges();
                    break;
                case "paid":
                    var orders = db.Orders.SingleOrDefault(e => e.OId == OId);
                    orders.Paid = true;
                    db.SaveChanges();
                    break;
            }
            return Redirect(returnUrl);
        }
    }
}