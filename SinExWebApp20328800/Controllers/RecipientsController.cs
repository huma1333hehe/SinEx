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
    public class RecipientsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: Recipients
        public ActionResult Index()
        {
            var recipients = db.Recipients.Include(r => r.ShippingAccount);
            return View(recipients.ToList());
        }

        // GET: Recipients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            return View(recipient);
        }

        // GET: Recipients/Create
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
        public ActionResult Create([Bind(Include = "RecipientID,FullName,CompanyName,DepartmentName,DeliveryAddress,PhoneNumber,Email,Nickname")] Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                //Get the current username 
                string username = System.Web.HttpContext.Current.User.Identity.Name;
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ShippingAccount account = db.ShippingAccounts.SingleOrDefault(s => s.UserName == username);
                if (account == null)
                {
                    return HttpNotFound("There is no account with user name \"" + username + "\".");
                }
                recipient.ShippingAccount = account;
                recipient.ShippingAccountId = account.ShippingAccountId;

                //check duplicate or not 
                bool general_duplicate = false;
                bool nickname_duplicate = false;
                IEnumerable<Recipient> exist = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);

                foreach (var s in exist)
                {
                    if (s.FullName == recipient.FullName && s.CompanyName == recipient.CompanyName && s.DeliveryAddress == recipient.DeliveryAddress && s.DepartmentName == recipient.DepartmentName && s.Email == recipient.Email && s.PhoneNumber == recipient.PhoneNumber)
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
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            if (recipient == null)
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
        public ActionResult Edit([Bind(Include = "RecipientID,FullName,CompanyName,DepartmentName,DeliveryAddress,PhoneNumber,Email,Nickname")] Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", recipient.ShippingAccountId);
            return View(recipient);
        }

        // GET: Recipients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipient recipient = db.Recipients.Find(id);
            if (recipient == null)
            {
                return HttpNotFound();
            }
            return View(recipient);
        }

        // POST: Recipients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}
