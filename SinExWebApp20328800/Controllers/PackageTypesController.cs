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
    public class PackageTypesController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: PackageTypes
        public ActionResult Index()
        {
            return View(db.PackageTypes.ToList());
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageType packageType = db.PackageTypes.Find(id);
            if (packageType == null)
            {
                return HttpNotFound();
            }
            return View(packageType);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypes/Create
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageTypeID,Type,Description")] PackageType packageType)
        {
            if (ModelState.IsValid)
            {
                db.PackageTypes.Add(packageType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(packageType);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageType packageType = db.PackageTypes.Find(id);
            if (packageType == null)
            {
                return HttpNotFound();
            }
            return View(packageType);
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageTypeID,Type,Description")] PackageType packageType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(packageType);
        }
        [Authorize(Roles = "Employee")]
        // GET: PackageTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageType packageType = db.PackageTypes.Find(id);
            if (packageType == null)
            {
                return HttpNotFound();
            }
            return View(packageType);
        }
        [Authorize(Roles = "Employee")]
        // POST: PackageTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PackageType packageType = db.PackageTypes.Find(id);
            db.PackageTypes.Remove(packageType);
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
