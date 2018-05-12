using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBookStore.Models;
using PagedList;
using PagedList.Mvc;

namespace MvcBookStore.Controllers
{
    public class HomeController : Controller
    {
        // tao 1 doi tuong chua toan bo CSDL
        dbQLBansachDataContext data = new dbQLBansachDataContext();
        // GET: BookStore
        private List<SACH> Laysachmoi(int count)
        {
            //sap xep dan theo ngay cap nhat, lay count dong dau
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }

        public ActionResult Index(int? page)
        {
            //tao bien quy dinh so san pham tren moi trang
            int pageSize = 5;
            //tao bien so trang
            int PageNum = (page ?? 1);

            // lay 5 quyen sach moi nhat
            var sachmoi = Laysachmoi(100);
            return View(sachmoi.ToPagedList(PageNum, pageSize));
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}