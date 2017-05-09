using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;
using System.Net.Mail;
using System.Threading.Tasks;

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
            var trackings = db.Trackings.Include(t => t.Shipment).OrderBy(s => s.WaybillId);
            if (WaybillId != null && WaybillId != 0)
            {
                Shipment shipment = db.Shipments.Single(a => a.WaybillId == WaybillId);
                trackings = trackings.Where(s => s.WaybillId == WaybillId).OrderByDescending(s => s.Time);
                ViewBag.ShowResult = true;
                if (shipment.DeliveredOrNot)
                {
                    ViewBag.delivered = true;
                    Tracking tracking = trackings.Single(j => j.Description == "Delivered" || j.Description == "Returned" || j.Description == "Lost");
                    ViewBag.DeliveredTo = tracking.DeliveredTo;
                    ViewBag.DeliveredAt = tracking.DeliveredAt;
                    ViewBag.StatusInformation = tracking.StatusInformation;
                }
                else
                {
                    ViewBag.delivered = false;
                }
            }
            else
            {
                ViewBag.ShowResult = false;
                ViewBag.delivered = false;
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
            if (tracking.Description == "Delivered" || tracking.Description == "Returned" || tracking.Description == "Lost")
            {
                ViewBag.delivered = true;
            }
            else
            {
                ViewBag.delivered = false;
            }
            return View(tracking);
        }

        // GET: Trackings/Create

        public ActionResult Create(int? WaybillId, bool? terminated)
        {

            if (WaybillId == null)
            {
                a = false;
                ViewBag.AlreadyEnterWaybillId = false;
                ViewBag.WaybillId = new SelectList(db.Shipments.Where(s => s.CancelledOrNot == false && s.PickupOrNot == true && s.DeliveredOrNot == false), "WaybillId", "WaybillId");
            }
            else
            {
                ViewBag.WaybillId = WaybillId;
                Shipment shipment = db.Shipments.SingleOrDefault(a => a.WaybillId == WaybillId && a.DeliveredOrNot == false);
                if (shipment == null)
                {
                    return View("CreateFail");
                }
                a = true;
                ViewBag.AlreadyEnterWaybillId = true;
            }
            if (terminated != null)
            {
                ViewBag.terminated = true;
            }
            return View();
        }

        // POST: Trackings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TrackingID,WaybillId,Time,Description,Location,Remark,DeliveredTo,DeliveredAt,StatusInformation")] Tracking tracking)
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
                ViewBag.WaybillId = new SelectList(db.Shipments.Where(s => s.CancelledOrNot == false && s.PickupOrNot == true && s.DeliveredOrNot == false), "WaybillId", "WaybillId", tracking.WaybillId);
                ViewBag.AlreadyEnterWaybillId = false;
            }

            if (tracking.Description == "Delivered" || tracking.Description == "Returned" || tracking.Description == "Lost")
            {
                shipment.DeliveredOrNot = true;
                shipment.DeliveredDate = tracking.Time;
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                db.Trackings.Add(tracking);
                db.SaveChanges();

                shipment.Trackings.Add(tracking);
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();

                //delivered notification email
                if (tracking.Description == "Delivered" && shipment.NotifySenderOrNot)
                {
                    var body_delivered = "<p>Dear user {0}: </p><p>The shipment with the following details has been delivered to the destination.</p><p>Shipment waybill Id: {1}</p><p>Delivered remark: {2}</p><p>Recipient name: {3}</p><p>Recipient address: {4}</p><p>Delivered date: {5}</p>";
                    var message_delivered = new MailMessage();
                    string Username = shipment.SenderShippingAccount.UserName;
                    string WaybillId = shipment.WaybillId.ToString("000000000000");
                    string Notification = tracking.Remark;
                    string RecipientName = "";
                    if (shipment.RecipientShippingAccount is PersonalShippingAccount)
                    {
                        PersonalShippingAccount b = (PersonalShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == shipment.RecipientShippingAccountID);
                        RecipientName = b.FirstName + b.LastName;
                    }
                    else
                    {
                        BusinessShippingAccount b = (BusinessShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == shipment.RecipientShippingAccountID);
                        RecipientName = b.ContactPersonName;
                    }
                    string RecipientAddress = shipment.RecipientShippingAccount.ProvinceCode + ", " + shipment.RecipientShippingAccount.City + ", " + shipment.RecipientShippingAccount.StreetInformation + ", " + shipment.RecipientShippingAccount.BuildingInformation;
                    string DeliveredDate = ((DateTime)(tracking.Time)).ToString("yyyy/MM/dd HH:mm");
                    message_delivered.To.Add(new MailAddress(shipment.SenderShippingAccount.EmailAddress));
                    message_delivered.Subject = "Delivered Notification Email";
                    message_delivered.Body = String.Format(body_delivered, Username, WaybillId, Notification, RecipientName, RecipientAddress, DeliveredDate);
                    message_delivered.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message_delivered);
                    }
                }
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
            if (tracking.Description == "Delivered" || tracking.Description == "Returned" || tracking.Description == "Lost")
            {
                ViewBag.delivered = true;
            }
            else
            {
                ViewBag.delivered = false;
            }
            return View(tracking);
        }

        // POST: Trackings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TrackingID,WaybillId,Time,Description,Location,Remark,DeliveredTo,DeliveredAt,StatusInformation")] Tracking tracking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tracking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.WaybillId = tracking.WaybillId;
            ViewBag.Time = tracking.Time;
            if (tracking.Description == "Delivered" || tracking.Description == "Returned" || tracking.Description == "Lost")
            {
                ViewBag.delivered = true;
            }
            else
            {
                ViewBag.delivered = false;
            }
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
            if (tracking.Description == "Delivered" || tracking.Description == "Returned" || tracking.Description == "Lost")
            {
                ViewBag.delivered = true;
            }
            else
            {
                ViewBag.delivered = false;
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
            shipment.DeliveredOrNot = false;
            shipment.DeliveredDate = null;
            shipment.Trackings.Remove(tracking);
            db.Entry(shipment).State = EntityState.Modified;
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