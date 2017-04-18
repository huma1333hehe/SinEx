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
    [Authorize(Roles = "Customer")]
    public class RecipientsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();


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

        // GET: Recipients
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Index()
        {
            var recipients = db.Recipients.Include(p => p.ShippingAccount);
            if (User.IsInRole("Customer"))
            {
                ShippingAccount shippingAccount = GetCurrentAccount();
                recipients = recipients.Where(s => s.ShippingAccountId == shippingAccount.ShippingAccountId);
            }
            return View(recipients.ToList());
        }

        // GET: Recipients/Details/5
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            ShippingAccount currentAccount = GetCurrentAccount();
            if (recipient == null || recipient.ShippingAccountId != currentAccount.ShippingAccountId)
            {
                return HttpNotFound();
            }
            return View(recipient);
        }

        // GET: Recipients/Create
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName");
            return View();
        }

        // POST: Recipients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "RecipientID,FullName,CompanyName,DepartmentName,DeliveryBuilding,DeliveryStreet,DeliveryCity,DeliveryProvince,DeliveryPostcode,PhoneNumber,Email,Nickname")] Recipient recipient)
        {


            if (ModelState.IsValid)
            {
                ShippingAccount account = GetCurrentAccount();
                recipient.ShippingAccount = account;
                recipient.ShippingAccountId = account.ShippingAccountId;


                //check duplicate or not 
                bool general_duplicate = false;
                bool nickname_duplicate = false;
                IEnumerable<Recipient> exist = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);

                foreach (var s in exist)
                {
                    if (s.FullName == recipient.FullName &&
                        s.CompanyName == recipient.CompanyName &&
                        s.DepartmentName == recipient.DepartmentName &&
                        s.DeliveryBuilding == recipient.DeliveryBuilding &&
                        s.DeliveryStreet == recipient.DeliveryStreet &&
                        s.DeliveryCity == recipient.DeliveryCity &&
                        s.DeliveryProvince == recipient.DeliveryProvince &&
                        s.DeliveryPostcode == recipient.DeliveryPostcode &&
                        s.Email == recipient.Email &&
                        s.PhoneNumber == recipient.PhoneNumber &&
                        s.DeliveryStreet == recipient.DeliveryStreet &&
                        s.DeliveryCity == recipient.DeliveryCity)
                    {
                        general_duplicate = true;
                        break;
                    }
                }

                foreach (var s in exist)
                {
                    if (s.Nickname == recipient.Nickname)
                    {
                        nickname_duplicate = true;
                        break;
                    }
                }
                ViewBag.general_duplicate = general_duplicate;
                ViewBag.nickname_duplicate = nickname_duplicate;

                if (!general_duplicate && !nickname_duplicate)
                {
                    db.Recipients.Add(recipient);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("CreateFail");
                }
            }
            return View(recipient);
        }


        // GET: Recipients/Edit/5
        [Authorize(Roles = "Customer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            ShippingAccount currentAccount = GetCurrentAccount();
            if (recipient == null || recipient.ShippingAccountId != currentAccount.ShippingAccountId)
            {
                return HttpNotFound();
            }
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", recipient.ShippingAccountId);
            return View(recipient);
        }

        // POST: Recipients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Edit([Bind(Include = "RecipientID,FullName,CompanyName,DepartmentName,DeliveryBuilding,DeliveryStreet,DeliveryCity,DeliveryProvince,DeliveryPostcode,PhoneNumber,Email,Nickname")] Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                ShippingAccount account = GetCurrentAccount();
                recipient.ShippingAccount = account;
                recipient.ShippingAccountId = account.ShippingAccountId;

                db.Entry(recipient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", recipient.ShippingAccountId);
            return View(recipient);
        }

        // GET: Recipients/Delete/5
        [Authorize(Roles = "Customer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            ShippingAccount currentAccount = GetCurrentAccount();
            if (recipient == null || recipient.ShippingAccountId != currentAccount.ShippingAccountId)
            {
                return HttpNotFound();
            }
            return View(recipient);
        }

        // POST: Recipients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipient recipient = db.Recipients.Find(id);
            db.Recipients.Remove(recipient);
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
        public ActionResult GetRecipientNickname(string Nickname)
        {
            if (string.IsNullOrEmpty(Nickname))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.Recipients.Where(a => a.ShippingAccountId == current_account.ShippingAccountId).Select(a => a.Nickname);
            if (hehe.Contains(Nickname))
            {
                return Json(current_account.UserName, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetGeneralRecipient(
            string FullName,
            string CompanyName,
            string DepartmentName,
            string DeliveryBuilding,
            string DeliveryStreet,
            string DeliveryCity,
            string DeliveryProvince,
            string DeliveryPostcode,
            string PhoneNumber,
            string Email)
        {
            if (string.IsNullOrEmpty(FullName) ||
                string.IsNullOrEmpty(CompanyName) ||
                string.IsNullOrEmpty(DepartmentName) ||
                string.IsNullOrEmpty(DeliveryBuilding) ||
                string.IsNullOrEmpty(DeliveryStreet) ||
                string.IsNullOrEmpty(DeliveryCity) ||
                string.IsNullOrEmpty(DeliveryProvince) ||
                string.IsNullOrEmpty(DeliveryPostcode) ||
                string.IsNullOrEmpty(PhoneNumber) ||
                string.IsNullOrEmpty(Email))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.Recipients.Where(a => a.ShippingAccountId == current_account.ShippingAccountId
            && a.FullName == FullName
            && a.CompanyName == CompanyName
            && a.DepartmentName == DepartmentName
            && a.DeliveryBuilding == DeliveryBuilding
            && a.DeliveryStreet == DeliveryStreet
            && a.DeliveryCity == DeliveryCity
            && a.DeliveryProvince == DeliveryProvince
            && a.DeliveryPostcode == DeliveryPostcode
            && a.PhoneNumber == PhoneNumber
            && a.Email == Email).Select(a => a.ShippingAccountId);
            if (hehe.Contains(current_account.ShippingAccountId))
            {
                return Json(current_account.UserName, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

    }
}