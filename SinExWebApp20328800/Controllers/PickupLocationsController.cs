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
    public class PickupLocationsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: PickupLocations
        public ActionResult Index()
        {
            var pickupLocations = db.PickupLocations.Include(p => p.ShippingAccount);
            return View(pickupLocations.ToList());
        }

        // GET: PickupLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickupLocation pickupLocation = db.PickupLocations.Find(id);
            if (pickupLocation == null)
            {
                return HttpNotFound();
            }
            return View(pickupLocation);
        }

        // GET: PickupLocations/Create
        public ActionResult Create()
        {
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName");
            return View();
        }

        // POST: PickupLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PickupLocationID,ShippingAccountId,Nickname,Location")] PickupLocation pickupLocation)
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
                pickupLocation.ShippingAccount = account;
                pickupLocation.ShippingAccountId = account.ShippingAccountId;

                //check duplicate or not 
                bool general_duplicate = false;
                bool nickname_duplicate = false;
                IEnumerable<PickupLocation> exist = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == account.ShippingAccountId);

                foreach (var s in exist)
                {
                    if (s.Location == pickupLocation.Location)
                    {
                        general_duplicate = true;
                        break;
                    }
                }

                foreach (var s in exist)
                {
                    if (s.Nickname == pickupLocation.Nickname)
                    {
                        nickname_duplicate = true;
                        break;
                    }
                }
                ViewBag.general_duplicate = general_duplicate;
                ViewBag.nickname_duplicate = nickname_duplicate;
                if (!general_duplicate && !nickname_duplicate)
                {
                    db.PickupLocations.Add(pickupLocation);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("CreateFail");
                }

            }
            return View(pickupLocation);
        }

        // GET: PickupLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickupLocation pickupLocation = db.PickupLocations.Find(id);
            if (pickupLocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", pickupLocation.ShippingAccountId);
            return View(pickupLocation);
        }

        // POST: PickupLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PickupLocationID,ShippingAccountId,Nickname,Location")] PickupLocation pickupLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pickupLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ShippingAccountId = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", pickupLocation.ShippingAccountId);
            return View(pickupLocation);
        }

        // GET: PickupLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PickupLocation pickupLocation = db.PickupLocations.Find(id);
            if (pickupLocation == null)
            {
                return HttpNotFound();
            }
            return View(pickupLocation);
        }

        // POST: PickupLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PickupLocation pickupLocation = db.PickupLocations.Find(id);
            db.PickupLocations.Remove(pickupLocation);
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
