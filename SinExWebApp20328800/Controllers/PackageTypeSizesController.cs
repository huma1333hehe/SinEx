using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;

namespace SinExWebApp20328800.Controllers
{
    public class PackageTypeSizesController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: PackageTypeSizes
        public ActionResult Index()
        {
            var packageTypeSizes = db.PackageTypeSizes.Include(p => p.PackageType);
            return View(packageTypeSizes.ToList());
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypeSizes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageTypeSize packageTypeSize = db.PackageTypeSizes.Find(id);
            if (packageTypeSize == null)
            {
                return HttpNotFound();
            }
            return View(packageTypeSize);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypeSizes/Create
        public ActionResult Create()
        {
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type");
            return View();
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypeSizes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageTypeSizeID,TypeSize,WeightLimit,PackageTypeID")] PackageTypeSize packageTypeSize)
        {
            if (ModelState.IsValid)
            {
                db.PackageTypeSizes.Add(packageTypeSize);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", packageTypeSize.PackageTypeID);
            return View(packageTypeSize);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypeSizes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageTypeSize packageTypeSize = db.PackageTypeSizes.Find(id);
            if (packageTypeSize == null)
            {
                return HttpNotFound();
            }
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", packageTypeSize.PackageTypeID);
            return View(packageTypeSize);
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypeSizes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageTypeSizeID,TypeSize,WeightLimit,PackageTypeID")] PackageTypeSize packageTypeSize)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageTypeSize).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", packageTypeSize.PackageTypeID);
            return View(packageTypeSize);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypeSizes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageTypeSize packageTypeSize = db.PackageTypeSizes.Find(id);
            if (packageTypeSize == null)
            {
                return HttpNotFound();
            }
            return View(packageTypeSize);
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypeSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackageTypeSize packageTypeSize = db.PackageTypeSizes.Find(id);
            db.PackageTypeSizes.Remove(packageTypeSize);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Employee")]
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
