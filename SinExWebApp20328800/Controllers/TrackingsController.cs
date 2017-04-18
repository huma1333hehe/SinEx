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
    public class TrackingsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        bool a = true;
        //Get current account 
        private ShippingAccount GetCurrentAccount()
        {
            string username = System.Web.HttpContext.Current.User.Identity.Name;
            if (username == null)
            {
                return null;
            }
            ShippingAccount current_account = db.ShippingAccounts.SingleOrDefault(s => s.UserName == username);
            return current_account;
        }
        // GET: Trackings
        public ActionResult Index(int? WaybillId)
        {
            ViewBag.shipments = db.Shipments.Where(s => s.CancelledOrNot == false && s.PickupOrNot == true).ToList();
            ViewBag.shipment = WaybillId;
            var trackings = db.Trackings.Include(t => t.Shipment);
            if (WaybillId != null)
            {
                trackings = trackings.Where(t => t.Shipment.WaybillId == WaybillId);
            }
            return View(trackings.ToList());
        }
        // GET: Trackings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tracking tracking = db.Trackings.Find(id);
            if (tracking == null)
            {
                return HttpNotFound();
            }
            return View(tracking);
        }

        // GET: Trackings/Create
        public ActionResult Create(int? WaybillId)
        {
            if (WaybillId == null)
            {
                a = false;
                ViewBag.AlreadyEnterWaybillId = false;
                ViewBag.WaybillId = new SelectList(db.Shipments.Where(s => s.CancelledOrNot == false && s.PickupOrNot == true), "WaybillId", "WaybillId");
            }
            else
            {
                a = true;
                ViewBag.AlreadyEnterWaybillId = true;
                ViewBag.WaybillId = WaybillId;
            }

            return View();
        }

        // POST: Trackings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TrackingID,WaybillId,Time,Description,Location,Remark")] Tracking tracking)
        {
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == tracking.WaybillId && s.CancelledOrNot == false);
            tracking.Shipment = shipment;
            if (a)
            {
                ViewBag.AlreadyEnterWaybillId = true;
                ViewBag.WaybillId = tracking.WaybillId;
            }
            else
            {
                ViewBag.WaybillId = new SelectList(db.Shipments, "WaybillId", "WaybillId", tracking.WaybillId);
                ViewBag.AlreadyEnterWaybillId = false;
            }
            if (ModelState.IsValid)
            {
                shipment.Trackings.Add(tracking);
                //db.Trackings.Add(tracking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tracking);

        }

        // GET: Trackings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tracking tracking = db.Trackings.Find(id);
            if (tracking == null)
            {
                return HttpNotFound();
            }
            ViewBag.WaybillId = tracking.WaybillId;
            ViewBag.Time = tracking.Time;
            return View(tracking);
        }

        // POST: Trackings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TrackingID,WaybillId,Time,Description,Location,Remark")] Tracking tracking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tracking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WaybillId = tracking.WaybillId;
            ViewBag.Time = tracking.Time;
            return View(tracking);
        }

        // GET: Trackings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tracking tracking = db.Trackings.Find(id);
            if (tracking == null)
            {
                return HttpNotFound();
            }
            return View(tracking);
        }

        // POST: Trackings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Tracking tracking = db.Trackings.Find(id);
            Shipment shipment = db.Shipments.Single(s => s.WaybillId == tracking.WaybillId && s.CancelledOrNot == false);
            //shipment.Trackings.Remove(tracking);
            db.Trackings.Remove(tracking);
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

        public ActionResult GetTime(DateTime Time, int WaybillId)
        {
            if (string.IsNullOrEmpty(Time.ToString()))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.Trackings.Where(a => a.WaybillId == WaybillId).Select(a => a.Time).OrderBy(a => a);
            bool faultornot = false;
            DateTime haha = new DateTime();
            foreach (DateTime hihi in hehe)
            {
                if (Time <= hihi)
                {
                    faultornot = true;
                    haha = hihi;
                    break;
                }
            }
            if (faultornot)
            {
                return Json(haha, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}