using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBookStore.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        dbQLBansachDataContext db = new dbQLBansachDataContext();
        //
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            //return View(db.SACHes.ToList());
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber,pageSize));
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe),"MaCD","TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSach(SACH sach, HttpPostedFileBase fileupload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            if (fileupload == null)
            {
                ViewBag.Thongbap = "Vui lòng chọn ản bìa";
                return View();
            }
            else
            {
                if(ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("/Content/Hinhsanpham"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.ThongBao = "Hinh anh ton tai";
                    else
                    {
                        fileupload.SaveAs( path);
                    }
                    sach.Anhbia = fileName;
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();

                }
                return RedirectToAction("Sach");
            }
            
        }
        public ActionResult ChiTietSach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if(sach == null)
            {

                Response.StatusCode = 404;
                return null;
            }

            return View(sach);
        }

        // chinh sua san pham
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //Lay doi tuong sach theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            //Dua du lieu vao dropDownList
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (ModelState.IsValid)
            {
                SACH objSach = db.SACHes.SingleOrDefault(n => n.Masach == sach.Masach);
                if (fileUpload != null)
                {
                    // Luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("Content/Hinhsanpham"), fileName);
                    //Kiem tra hing anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    objSach.Anhbia = fileName;
                }
                objSach.Ngaycapnhat = DateTime.Now;
                objSach.Soluongton = sach.Soluongton;
                objSach.Mota = sach.Mota;
                objSach.MaNXB = sach.MaNXB;
                objSach.Tensach = sach.Tensach;
                objSach.MaCD = sach.MaCD;
                objSach.Giaban = sach.Giaban;
                //Luu vao CSD
                UpdateModel(objSach);
                db.SubmitChanges();
            }
            return RedirectToAction("sach");
        }
        //xoa san pham
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            SACH sach =db.SACHes.SingleOrDefault(n => n.Masach == id) ;
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gán giá trị nguoi dùng nhap lieu cho cac bien
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //gán giá trị cho đối tượng được tạo mới(ad)
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassWord == matkhau);
                if (ad != null)
                {
                    // ViewBag.Thongbao ="Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"]= ad;
                    return RedirectToAction("Index","Admin");

                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
    }
}