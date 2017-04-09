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
    public class PackagesController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: Packages
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Index(
            int WaybillId
            )
        {
            ViewBag.Waybillid = WaybillId;
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == WaybillId);
            if (User.IsInRole("Customer"))
            {
                ShippingAccount shippingAccount = db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
                //check if the waybill belongs to the current user
                if (shipment.SenderShippingAccountID != shippingAccount.ShippingAccountId)
                {
                    return RedirectToAction("Shipments", "index");
                }
            }
            //get packages from database
            var packages = db.Packages.Include(p => p.Currency).Include(p => p.PackageType).Include(p => p.Shipment).Where(p => p.WaybillId == shipment.WaybillId);
            return View(packages.ToList());
        }

        // GET: Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: Packages/Create
        public ActionResult Create(
            int WaybillId
            )
        {
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == WaybillId);
            if (shipment.Packages.Count() >= 10)
            {
                return RedirectToAction("Details", "Shipments", new { id = WaybillId });
            }

            ViewBag.CurrencyCode = new SelectList(db.Currencies, "CurrencyCode", "CurrencyCode");
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type");
            ViewBag.PackageTypeSizeID = new SelectList(db.PackageTypeSizes.Where(s => s.PackageTypeSizeID == 0), "PackageTypeSizeID", "TypeSize");
            ViewBag.WaybillId = WaybillId;
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageID,WaybillId,PackageTypeID,Description,Value,CurrencyCode,DeclaredWeight,ActualWeight,PackageTypeSizeID")] Package package)
        {
            package.ActualWeight = null;
            if (ModelState.IsValid)
            {
                db.Packages.Add(package);
                Shipment shipment = db.Shipments.Single(s => s.WaybillId == package.WaybillId);
                shipment.NumberOfPackages += 1;
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { WaybillId = package.WaybillId });
            }

            ViewBag.CurrencyCode = new SelectList(db.Currencies, "CurrencyCode", "CurrencyCode", package.CurrencyCode);
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", package.PackageTypeID);
            ViewBag.PackageTypeSizeID = new SelectList(db.PackageTypeSizes.Where(s => s.PackageTypeSizeID == 0), "PackageTypeSizeID", "TypeSize");
            ViewBag.WaybillId = package.WaybillId;
            return View(package);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrencyCode = new SelectList(db.Currencies, "CurrencyCode", "CurrencyCode", package.CurrencyCode);
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", package.PackageTypeID);
            ViewBag.PackageTypeSizeID = new SelectList(db.PackageTypeSizes.Where(s => s.PackageTypeID == package.PackageTypeID), "PackageTypeSizeID", "TypeSize", package.PackageTypeSizeID);
            ViewBag.WaybillId = package.WaybillId;
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageID,WaybillId,PackageTypeID,Description,Value,CurrencyCode,DeclaredWeight,ActualWeight,PackageTypeSizeID")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Entry(package).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { WaybillId = package.WaybillId });
            }
            ViewBag.CurrencyCode = new SelectList(db.Currencies, "CurrencyCode", "CurrencyCode", package.CurrencyCode);
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", package.PackageTypeID);
            ViewBag.PackageTypeSizeID = new SelectList(db.PackageTypeSizes.Where(s => s.PackageTypeID == package.PackageTypeID), "PackageTypeSizeID", "TypeSize", package.PackageTypeSizeID);
            ViewBag.WaybillId = package.WaybillId;
            return View(package);
        }

        // GET: Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.Packages.Find(id);
            db.Packages.Remove(package);
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == package.WaybillId);
            shipment.NumberOfPackages -= 1;
            db.Entry(shipment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { WaybillId = package.WaybillId });
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
