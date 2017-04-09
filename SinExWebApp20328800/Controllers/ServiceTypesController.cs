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
    public class ServiceTypesController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: ServiceTypes
        public ActionResult Index()
        {
            return View(db.ServiceTypes.ToList());
        }
        [Authorize(Roles = "Employee")]
        // GET: ServiceTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceType serviceType = db.ServiceTypes.Find(id);
            if (serviceType == null)
            {
                return HttpNotFound();
            }
            return View(serviceType);
        }
        [Authorize(Roles = "Employee")]
        // GET: ServiceTypes/Create
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Employee")]
        // POST: ServiceTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ServiceTypeID,Type,CutoffTime,DeliveryTime")] ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                db.ServiceTypes.Add(serviceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviceType);
        }
        [Authorize(Roles = "Employee")]
        // GET: ServiceTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceType serviceType = db.ServiceTypes.Find(id);
            if (serviceType == null)
            {
                return HttpNotFound();
            }
            return View(serviceType);
        }
        [Authorize(Roles = "Employee")]
        // POST: ServiceTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ServiceTypeID,Type,CutoffTime,DeliveryTime")] ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(serviceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(serviceType);
        }
        [Authorize(Roles = "Employee")]
        // GET: ServiceTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceType serviceType = db.ServiceTypes.Find(id);
            if (serviceType == null)
            {
                return HttpNotFound();
            }
            return View(serviceType);
        }
        [Authorize(Roles = "Employee")]
        // POST: ServiceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceType serviceType = db.ServiceTypes.Find(id);
            db.ServiceTypes.Remove(serviceType);
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
