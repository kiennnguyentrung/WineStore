using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ViDuThucHanh.Models;
using PagedList;

namespace ViDuThucHanh.Controllers
{
    public class ProductsController : Controller
    {
        private WineStoreDB db = new WineStoreDB();

        // GET: Products
        public ActionResult Index(string sortOrder, string searchString, string currenFilter, int? page)
        {
            // các biến sắp xếp
            ViewBag.CurrentSort = sortOrder;//Biến lấy yêu cầu sắp xếp hiện tại
            ViewBag.SapTheoTen = String.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";
            ViewBag.SapTheoGia = sortOrder == "gia" ? "gia_desc" : "gia";
            //Lấy giá trị của bộ lọc dữ liệu hiện tại
            if(searchString != null)
            {
                page = 1; //trang đầu tiên
            }    
            else
            {
                searchString = currenFilter;
            }
            ViewBag.CurrentFilter = searchString;

            // lấy danh sách hàng
            var products = db.Products.Select(p => p);
            // lọc theo tên hàng
            if(!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }    
            //sắp xếp
            switch (sortOrder)
            {
                case "ten_desc":
                    products = products.OrderByDescending(s => s.ProductName);
                    break;
                case "gia":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "gia_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.ProductName);
                    break;
            }
            int pageSize = 3;//kích thước trang
            int pageNumber = (page ?? 1);//nếu page bằng null thì trả về 1
            return View(products.ToPagedList(pageNumber,pageSize));
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Description,PurchasePrice,Price,Quantity,Vintage,CatalogyID,Image,Region")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                { 
                    product.Image = "";
                    var f = Request.Files["ImageFile"];
                    if(f != null&& f.ContentLength>0)
                    {
                        //user namespace called: system.IO
                        string FileName = System.IO.Path.GetFileName(f.FileName);
                        //lấy tên file upload
                        string UploadPath = Server.MapPath("~/wwwroot/" + FileName);
                        //copy và lưu file vào server
                        f.SaveAs(UploadPath);
                        //lưu tên file vào trường Image
                        product.Image = FileName;
                    }    
                    db.Products.Add(product);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi nhập dữ liệu!" + ex.Message;
                ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
                return View(product);
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Description,PurchasePrice,Price,Quantity,Vintage,CatalogyID,Image,Region")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.Image = "";
                    var f = Request.Files["ImageFile"];
                    if (f != null && f.ContentLength > 0)
                    {
                        string FileName = System.IO.Path.GetFileName(f.FileName);
                        string UploadPath = Server.MapPath("~/wwwroot/" + FileName);
                        f.SaveAs(UploadPath);
                        product.Image = FileName;
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi sửa dữ liệu" + ex.Message;
                return View(product);
            }
            //ViewBag.CatalogyID = new SelectList(db.Catalogies, "CatalogyID", "CatalogyName", product.CatalogyID);
            //return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
