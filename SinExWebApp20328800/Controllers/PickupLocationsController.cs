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


        // GET: PickupLocations
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Index()
        {
            var pickupLocations = db.PickupLocations.Include(p => p.ShippingAccount);
            if (User.IsInRole("Customer"))
            {
                ShippingAccount shippingAccount = GetCurrentAccount();
                pickupLocations = pickupLocations.Where(s => s.ShippingAccountId == shippingAccount.ShippingAccountId);
            }
            return View(pickupLocations.ToList());
        }

        // GET: PickupLocations/Details/5
        [Authorize(Roles = "Customer,Employee")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "PickupLocationID,ShippingAccountId,Nickname,Location")] PickupLocation pickupLocation)
        {
            if (ModelState.IsValid)
            {
                ShippingAccount account = GetCurrentAccount();
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

        public ActionResult GetPickupLocationNickname(string Nickname)
        {
            if (string.IsNullOrEmpty(Nickname))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.PickupLocations.Where(a => a.ShippingAccountId == current_account.ShippingAccountId).Select(a => a.Nickname);
            if (hehe.Contains(Nickname))
            {
                return Json(current_account.UserName, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocation(string Location)
        {
            if (string.IsNullOrEmpty(Location))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.PickupLocations.Where(a => a.ShippingAccountId == current_account.ShippingAccountId).Select(a => a.Location);
            if (hehe.Contains(Location))
            {
                return Json(current_account.UserName, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        // GET: PickupLocations/Edit/5
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
        [Authorize(Roles = "Customer")]
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
