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
            if (shipment == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.CancelledOrNot = shipment.CancelledOrNot;
                ViewBag.ConfirmOrNot = shipment.ConfirmOrNot;
            }
            if (User.IsInRole("Customer"))
            {
                ShippingAccount shippingAccount = db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
                //check if the waybill belongs to the current user
                if (shipment.SenderShippingAccountID != shippingAccount.ShippingAccountId)
                {
                    return HttpNotFound();
                }
            }
            //get packages from database
            var packages = db.Packages.Include(p => p.Currency).Include(p => p.PackageType).Include(p => p.Shipment).Where(p => p.WaybillId == shipment.WaybillId);
            return View(packages.ToList());
        }

        // GET: Packages/Details/5
        [Authorize(Roles = "Customer,Employee")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult Create(
            int WaybillId
            )
        {
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == WaybillId && s.CancelledOrNot == false && s.ConfirmOrNot == false);
            if (shipment == null)
            {
                return HttpNotFound();
            }
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
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PackageID,WaybillId,PackageTypeID,Description,Value,CurrencyCode,DeclaredWeight,ActualWeight,PackageTypeSizeID")] Package package)
        {
            package.DeclaredWeight = Math.Round(package.DeclaredWeight, 1);
            package.ActualWeight = null;
            bool isPackageTypeSizeEmpty = false;
            if (package.PackageTypeSizeID == 0)
            {
                isPackageTypeSizeEmpty = true;
            }
            if (ModelState.IsValid && isPackageTypeSizeEmpty==false)
            {   
                package.Currency = db.Currencies.Single(s => s.CurrencyCode == package.CurrencyCode);
                package.PackageType = db.PackageTypes.Single(s => s.PackageTypeID == package.PackageTypeID);
                package.PackageTypeSize = db.PackageTypeSizes.Single(s => s.PackageTypeSizeID == package.PackageTypeSizeID);
                package.Shipment = db.Shipments.Single(s => s.WaybillId == package.WaybillId);
                package.DeclaredFee = CalculatePackageFee(package);
                package.ActualFee = null;
                db.Packages.Add(package);
                db.SaveChanges();
                
                Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == package.WaybillId && s.CancelledOrNot == false);
                
                shipment.Packages.Add(package);
                shipment.NumberOfPackages += 1;
                shipment.EstimatedShipmentTotalAmount += package.DeclaredFee;
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { WaybillId = package.WaybillId });
            }
            if(isPackageTypeSizeEmpty == true)
            {
                ViewBag.PackageTypeSizeIDEmpty = "The Package Type Size field is required.";
            }

            ViewBag.CurrencyCode = new SelectList(db.Currencies, "CurrencyCode", "CurrencyCode", package.CurrencyCode);
            ViewBag.PackageTypeID = new SelectList(db.PackageTypes, "PackageTypeID", "Type", package.PackageTypeID);
            ViewBag.PackageTypeSizeID = new SelectList(db.PackageTypeSizes.Where(s => s.PackageTypeSizeID == 0), "PackageTypeSizeID", "TypeSize");
            ViewBag.WaybillId = package.WaybillId;
            return View(package);
        }

        // GET: Packages/Edit/5
        [Authorize(Roles = "Customer")]
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
            if (package.Shipment.CancelledOrNot == true || package.Shipment.ConfirmOrNot == true)
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
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PackageID,WaybillId,PackageTypeID,Description,Value,CurrencyCode,DeclaredWeight,ActualWeight,PackageTypeSizeID")] Package package)
        {
            package.DeclaredWeight = Math.Round(package.DeclaredWeight, 1);
            if (ModelState.IsValid)
            {
                Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == package.WaybillId && s.CancelledOrNot == false);
                package.Shipment = shipment;
                package.DeclaredFee = CalculatePackageFee(package);
                package.ActualFee = null;
                db.Entry(package).State = EntityState.Modified;
                db.SaveChanges();


                shipment.EstimatedShipmentTotalAmount = 0;
                foreach(Package p in shipment.Packages)
                {
                    shipment.EstimatedShipmentTotalAmount += CalculatePackageFee(p);
                }
                db.Entry(shipment).State = EntityState.Modified;
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
        [Authorize(Roles = "Customer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == package.WaybillId && s.CancelledOrNot == false);
            if (package == null || shipment == null)
            {
                return HttpNotFound();
            }
            if (package.Shipment.CancelledOrNot == true || package.Shipment.ConfirmOrNot == true)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [Authorize(Roles = "Customer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.Packages.Find(id);
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == package.WaybillId && s.CancelledOrNot == false);
            db.Packages.Remove(package);
            shipment.NumberOfPackages -= 1;
            shipment.EstimatedShipmentTotalAmount -= package.DeclaredFee;
            db.Entry(shipment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { WaybillId = package.WaybillId });
        }
        [Authorize(Roles = "Customer")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Calculate package fee from declared weight in RMB
        private decimal CalculatePackageFee(Package Package)
        {
            ServicePackageFee hehe = db.ServicePackageFees.SingleOrDefault(a => a.PackageTypeID == Package.PackageTypeID && a.ServiceTypeID == Package.Shipment.ServiceTypeID);
            PackageTypeSize haha = db.PackageTypeSizes.Single(a => a.PackageTypeSizeID == Package.PackageTypeSizeID);
            decimal price = 0;
            switch (Package.PackageTypeID)
            {
                //Envelop
                case 1:
                    price = hehe.Fee;
                    break;
                //Pak or Box
                case 2:
                case 4:
                    price = Package.DeclaredWeight * hehe.Fee > hehe.MinimumFee ? (decimal)Package.DeclaredWeight * hehe.Fee : hehe.MinimumFee;
                    int limit = 0;
                    string limitString = haha.WeightLimit;
                    bool convertResult = Int32.TryParse(limitString.Substring(0, limitString.Length - 2), out limit);
                    if (limit != 0 && convertResult && Package.DeclaredWeight > (decimal)limit)
                    {
                        price += 500;
                    }

                    break;
                //Tube or Custmoer
                case 3:
                case 5:
                    price = Package.DeclaredWeight * hehe.Fee > hehe.MinimumFee ? (decimal)Package.DeclaredWeight * hehe.Fee : hehe.MinimumFee;
                    break;
            }
            return price;
        }
    }
}
