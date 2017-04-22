using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SinExWebApp20328800.Models;
using SinExWebApp20328800.ViewModels;
using X.PagedList;

namespace SinExWebApp20328800.Controllers
{
    [Authorize(Roles = "Customer,Employee")]
    public class ShipmentsController : Controller
    {
        private SinExWebApp20328800DatabaseContext db = new SinExWebApp20328800DatabaseContext();

        // GET: Shipments
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult Index()
        {
            var shipments = db.Shipments.Include(s => s.RecipientShippingAccount).Include(s => s.SenderShippingAccount).Include(s => s.ServiceType);
            if (User.IsInRole("Customer"))
            {
                ShippingAccount shippingAccount = GetCurrentAccount();
                shipments = shipments.Where(s => s.SenderShippingAccountID == shippingAccount.ShippingAccountId);
            }
            return View(shipments.ToList());
        }

        // GET: Shipments/GenerateHistoryReport
        [Authorize(Roles = "Customer,Employee")]
        public ActionResult GenerateHistoryReport(
            int? ShippingAccountId,
            DateType? DateType,
            DateTime? StartingDate,
            DateTime? EndingDate,
            string sortOrder,
            int? currentShippingAccountId,
            DateType? currentDateType,
            DateTime? currentStartingDate,
            DateTime? currentEndingDate,
            int? page
            )
        {
            // Instantiate an instance of the ShipmentsReportViewModel and the ShipmentsSearchViewModel.
            var shipmentSearch = new ShipmentsReportViewModel();
            shipmentSearch.Shipment = new ShipmentsSearchViewModel();

            // Code for paging.
            ViewBag.CurrentSort = sortOrder;
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // Retain search conditions for sorting.
            if (DateType == null)
            {
                DateType = currentDateType;
            }
            if (StartingDate == null)
            {
                StartingDate = currentStartingDate;
            }
            if (EndingDate == null)
            {
                EndingDate = currentEndingDate;
            }
            if (ShippingAccountId == null)
            {
                ShippingAccountId = currentShippingAccountId;
            }
            else
            {
                page = 1;
            }

            ViewBag.currentShippingAccountId = ShippingAccountId;
            ViewBag.CurrentStartingDate = StartingDate;
            ViewBag.CurrentEndingDate = EndingDate;
            ViewBag.CurrentDateType = DateType;


            // Populate the ShippingAccountId dropdown list.
            shipmentSearch.Shipment.ShippingAccounts = PopulateShippingAccountsDropdownList().ToList();
            shipmentSearch.Shipment.ShippingAccounts.Insert(0, new SelectListItem
            {
                Text = "All",
                Value = null,
                Selected = false
            });

            //Initialize the query to retrieve shipments using the ShipmentsListViewModel.
            var shipmentQuery = from s in db.Shipments
                                select new ShipmentsListViewModel
                                {
                                    WaybillId = s.WaybillId,
                                    ServiceType = s.ServiceType.Type,
                                    ShippedDate = s.PickupDate,
                                    DeliveredDate = s.DeliveredDate,
                                    RecipientName = s.RecipientFullName,
                                    NumberOfPackages = s.NumberOfPackages,
                                    Origin = s.Origin,
                                    Destination = s.Destination,
                                    ShippingAccountId = s.SenderShippingAccountID,
                                };


            if (User.IsInRole("Customer"))
            {
                ShippingAccount CShippingAccount = db.ShippingAccounts.SingleOrDefault(s => s.UserName == User.Identity.Name);
                shipmentQuery = shipmentQuery.Where(s => s.ShippingAccountId == CShippingAccount.ShippingAccountId);
            }

            // Add the condition to select a spefic shipping account if shipping account id is not null.
            if (ShippingAccountId != null)
            {
                // TODO: Construct the LINQ query to retrive only the shipments for the specified shipping account id.
                shipmentQuery = shipmentQuery.Where(s => s.ShippingAccountId == ShippingAccountId);

            }
            if ((StartingDate != null) && (EndingDate != null) && DateType!=null)
            {
                if (DateType == ViewModels.DateType.ShippedDate)
                {
                    shipmentQuery = shipmentQuery.Where(s => (s.ShippedDate > StartingDate && s.ShippedDate < EndingDate));
                }
                if (DateType == ViewModels.DateType.DeliveredDate)
                {
                    shipmentQuery = shipmentQuery.Where(s => (s.DeliveredDate > StartingDate && s.DeliveredDate < EndingDate));
                }
            }
            else
            {
                // Return an empty result if no shipping account id has been selected.
                shipmentSearch.Shipments = new ShipmentsListViewModel[0].ToPagedList(pageNumber, pageSize);

            }

            // Code for sorting on ServiceType, ShippedDate, DeliveredDate, RecipientName, Origin, Destination
            ViewBag.ServiceTypeSortParm = sortOrder == "serviceType" ? "serviceType_desc" : "serviceType";
            ViewBag.ShippedDateSortParm = sortOrder == "shippedDate" ? "shippedDate_desc" : "shippedDate";
            ViewBag.DeliveredDateSortParm = sortOrder == "deliveredDate" ? "deliveredDate_desc" : "deliveredDate";
            ViewBag.RecipientNameSortParm = sortOrder == "recipientName" ? "recipientName_desc" : "recipientName";
            ViewBag.OriginSortParm = sortOrder == "origin" ? "origin_desc" : "origin";
            ViewBag.DestinationSortParm = sortOrder == "destination" ? "destination_desc" : "destination";
            switch (sortOrder)
            {
                case "serviceType_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.ServiceType);
                    break;
                case "serviceType":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.ServiceType);
                    break;
                case "shippedDate_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.ShippedDate);
                    break;
                case "shippedDate":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.ShippedDate);
                    break;
                case "deliveredDate_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.DeliveredDate);
                    break;
                case "deliveredDate":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.DeliveredDate);
                    break;
                case "recipientName_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.RecipientName);
                    break;
                case "recipientName":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.RecipientName);
                    break;
                case "origin_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.Origin);
                    break;
                case "origin":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.Origin);
                    break;
                case "destination_desc":
                    shipmentQuery = shipmentQuery.OrderByDescending(s => s.Destination);
                    break;
                case "destination":
                    shipmentQuery = shipmentQuery.OrderBy(s => s.Destination);
                    break;
                default:
                    shipmentQuery = shipmentQuery.OrderBy(s => s.WaybillId);
                    break;
            }
            shipmentSearch.Shipments = shipmentQuery.ToPagedList(pageNumber, pageSize);


            return View(shipmentSearch);
        }

        private SelectList PopulateShippingAccountsDropdownList()
        {
            // TODO: Construct the LINQ query to retrieve the unique list of shipping account ids.
            var shippingAccountQuery = db.Shipments.Select(s => s.SenderShippingAccountID).Distinct().OrderBy(s => s);
            return new SelectList(shippingAccountQuery);
        }
        // GET: Shipments/MultipleConfirm
        [Authorize(Roles = "Customer")]
        public ActionResult MultipleConfirm()
        {
            var shipments = db.Shipments.Include(s => s.RecipientShippingAccount).Include(s => s.SenderShippingAccount).Include(s => s.ServiceType).
                Where(s => s.CancelledOrNot == false && s.ConfirmOrNot == false && s.PickupOrNot == false && s.NumberOfPackages > 0);
            ShippingAccount current_account = GetCurrentAccount();
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipments.ToList());
        }

        // POST: Shipments/MultipleConfirm
        [HttpPost, ActionName("MultipleConfirm")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult MultipleConfirm(IEnumerable<Shipment> shipments)
        {
            List<string> s = new List<string>();
            if (shipments != null)
            {
                s = Request.Form["Shipments"].Split(',').ToList();
            }

            if (s.Count > 0)
            {
                foreach (string WaybillId in s)
                {
                    Shipment shipment = db.Shipments.Find(Int32.Parse(WaybillId));
                    shipment.PickupType = Request.Form["PickupType"].Equals("0") ? PickupType.Immediate : PickupType.Prearranged;
                    shipment.PickupLocation = Request.Form["PickupLocation"];
                    DateTime dt = Convert.ToDateTime(Request.Form["PickupDate"]);
                    shipment.PickupDate = dt;
                    shipment.PickupLocation = Request.Form["PickupLocation"];
                    shipment.ConfirmOrNot = true;
                    shipment.PickupOrNot = false;
                    db.Entry(shipment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ShippingAccount current_account = GetCurrentAccount();
            IEnumerable<PickupLocation> lala = db.PickupLocations.Where(x => x.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            var allShipments = db.Shipments.Include(a => a.RecipientShippingAccount).Include(a => a.SenderShippingAccount).Include(a => a.ServiceType).
                     Where(a => a.CancelledOrNot == false && a.ConfirmOrNot == false && a.PickupOrNot == false && a.NumberOfPackages > 0);
            return View(allShipments.ToList());
            /*
            foreach(string shipmentWaybillId in shipmentWaybillIds)
            {

            }


            return RedirectToAction("Index");

            /*
            shipment.ConfirmOrNot = true;
            shipment.PickupOrNot = false;
            db.Entry(shipment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
            */
        }



        // GET: Shipments/Details/5
        [Authorize(Roles = "Employee,Customer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Customer") && shipment.SenderShippingAccountID != GetCurrentAccount().ShippingAccountId)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // GET: Shipments/Create
        [Authorize(Roles = "Customer")]
        public ActionResult Create()
        {
            Shipment shipment = new Shipment();
            shipment.RecipientShippingAccountID = GetCurrentAccount().ShippingAccountId;
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type");
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");

            ShippingAccount current_account = GetCurrentAccount();

            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipment);
        }



        // POST: Shipments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Create([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,RecipientCreditCardNumber,RecipientCreditCardType,RecipientCreditCardSecurityNumber,RecipientCreditCardHolderName,RecipientCreditCardExpiryMonth,RecipientCreditCardExpiryYear")] Shipment shipment)
        {
            ShippingAccount current_account = GetCurrentAccount();

            shipment.SenderShippingAccountID = current_account.ShippingAccountId;
            shipment.ConfirmOrNot = false;
            shipment.PickupOrNot = false;
            shipment.CancelledOrNot = false;
            shipment.Duty = null;
            shipment.Tax = null;
            shipment.PickupType = null;
            shipment.PickupDate = null;
            shipment.NumberOfPackages = 0;

            bool is_recipient_valid = false;
            if (!(shipment.TaxPayer == TaxPayer.Recipient || shipment.ShipmentPayer == ShipmentPayer.Recipient))
            {
                shipment.RecipientShippingAccountID = current_account.ShippingAccountId;

                is_recipient_valid = true;
            }
            else
            {
                ShippingAccount recipient_account = db.ShippingAccounts.SingleOrDefault(s => s.ShippingAccountId == shipment.RecipientShippingAccountID);
                if (recipient_account != null && recipient_account != current_account)
                {
                    shipment.RecipientShippingAccountID = recipient_account.ShippingAccountId;
                    is_recipient_valid = true;
                }
                else if (recipient_account == null)
                {
                    is_recipient_valid = false;
                    ViewBag.ErrorMsg = "Recipient account ID does not exist, please input again!";
                }
                else if (recipient_account == current_account)
                {
                    is_recipient_valid = false;
                    ViewBag.ErrorMsg = "Recipient account ID can not be yours, please input again!";
                }
            }


            if (ModelState.IsValid && is_recipient_valid == true)
            {
                db.Shipments.Add(shipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.RecipientShippingAccountID = GetCurrentAccount().ShippingAccountId;
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.SenderShippingAccountID);
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type", shipment.ServiceTypeID);
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");

            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipment);
        }

        // GET: Shipments/Edit/5
        [Authorize(Roles = "Customer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false && s.ConfirmOrNot == false);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            if (shipment.SenderShippingAccountID != GetCurrentAccount().ShippingAccountId)
            {
                return HttpNotFound();
            }
            ViewBag.SenderShippingAccountID = shipment.RecipientShippingAccountID;
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type");
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");

            ShippingAccount current_account = GetCurrentAccount();

            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipment);
        }

        // POST: Shipments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Edit([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,RecipientCreditCardNumber,RecipientCreditCardType,RecipientCreditCardSecurityNumber,RecipientCreditCardHolderName,RecipientCreditCardExpiryMonth,RecipientCreditCardExpiryYear")] Shipment shipment)
        {
            shipment.ServiceType = db.ServiceTypes.SingleOrDefault(s => s.ServiceTypeID == shipment.ServiceTypeID);

            ShippingAccount current_account = GetCurrentAccount();

            bool is_recipient_valid = false;

            if (!(shipment.TaxPayer == TaxPayer.Recipient || shipment.ShipmentPayer == ShipmentPayer.Recipient))
            {
                shipment.RecipientShippingAccount = current_account;
                shipment.RecipientShippingAccountID = current_account.ShippingAccountId;

                is_recipient_valid = true;
            }
            else
            {
                ShippingAccount recipient_account = db.ShippingAccounts.SingleOrDefault(s => s.ShippingAccountId == shipment.RecipientShippingAccountID);
                if (recipient_account != null && recipient_account != current_account)
                {
                    shipment.RecipientShippingAccount = recipient_account;
                    shipment.RecipientShippingAccountID = recipient_account.ShippingAccountId;
                    is_recipient_valid = true;
                }
                else if (recipient_account == null)
                {
                    is_recipient_valid = false;
                    ViewBag.ErrorMsg = "Recipient account ID does not exist, please input again!";
                }
                else if (recipient_account == current_account)
                {
                    is_recipient_valid = false;
                    ViewBag.ErrorMsg = "Recipient account ID can not be yours, please input again!";
                }
            }

            if (ModelState.IsValid && is_recipient_valid == true)
            {
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.RecipientShippingAccountID = shipment.RecipientShippingAccountID;
            ViewBag.SenderShippingAccountID = new SelectList(db.ShippingAccounts, "ShippingAccountId", "UserName", shipment.SenderShippingAccountID);
            ViewBag.ServiceTypeID = new SelectList(db.ServiceTypes, "ServiceTypeID", "Type", shipment.ServiceTypeID);
            ViewBag.Origin = new SelectList(db.Destinations, "City", "City");
            ViewBag.Destination = new SelectList(db.Destinations, "City", "City");

            IEnumerable<Recipient> hehe = db.Recipients.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.RecipientAddressNickname = new SelectList(hehe, "Nickname", "Nickname");
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            return View(shipment);
        }

        // GET: Shipments/Delete/5
        [Authorize(Roles = "Customer")]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            if (shipment.SenderShippingAccountID != GetCurrentAccount().ShippingAccountId)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Delete/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Cancel(int id)
        {
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false);
            shipment.CancelledOrNot = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Shipments/Confirm/5
        [Authorize(Roles = "Customer")]
        public ActionResult Confirm(int? id)
        {
            ShippingAccount current_account = GetCurrentAccount();
            IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
            ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            if (shipment.SenderShippingAccountID != GetCurrentAccount().ShippingAccountId)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        // POST: Shipments/Confirm/5
        [HttpPost, ActionName("Confirm")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public ActionResult Confirm([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,RecipientCreditCardNumber,RecipientCreditCardType,RecipientCreditCardSecurityNumber,RecipientCreditCardHolderName,RecipientCreditCardExpiryMonth,RecipientCreditCardExpiryYear")] Shipment shipment)
        {
            shipment.ConfirmOrNot = true;
            shipment.PickupOrNot = false;
            db.Entry(shipment).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: Shipments/Pickup/5
        [Authorize(Roles = "Employee")]
        public ActionResult Pickup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Include(s => s.Packages).SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false);

            if (shipment == null)
            {
                return HttpNotFound();
            }
            if (shipment.Packages == null || shipment.NumberOfPackages == 0)
            {
                ViewBag.Error = "This shipment has no packages!";
            }
            if (shipment.PickupOrNot == true)
            {
                ViewBag.Error = "This shipment has been picked up!";
            }
            if (shipment.ConfirmOrNot == false)
            {
                ViewBag.Error = "This shipment has not been confirmed!";
            }
            return View(shipment);
        }

        // POST: Shipments/Pickup/5
        [HttpPost, ActionName("Pickup")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
        public ActionResult Pickup([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,RecipientCreditCardNumber,RecipientCreditCardType,RecipientCreditCardSecurityNumber,RecipientCreditCardHolderName,RecipientCreditCardExpiryMonth,RecipientCreditCardExpiryYear")] Shipment shipment)
        {

            db.Shipments.Attach(shipment);
            db.Entry(shipment).Collection(p => p.Packages).Load();

            Tracking tracking = new Tracking();
            tracking.WaybillId = shipment.WaybillId;
            bool validate = true;
            if (Request.Form["Duty"] == "" || Request.Form["Tax"] == "" || Request.Form["TrackingPickupLocation"] == "" || Request.Form["PickupRemark"] == "")
            {
                ViewBag.Incomplete = "Input Incomplete";
                validate = false;
            }

            foreach (Package package in shipment.Packages)
            {
                string key = "package_" + package.PackageID.ToString();
                if (Request.Form[key] != "")
                {
                    decimal value;
                    if (Decimal.TryParse(Request.Form[key], out value))
                    {
                        package.ActualWeight = value;
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Input must be decimal";
                        validate = false;
                    }

                }
                else
                {
                    ViewBag.Incomplete = "Input Incomplete";
                    validate = false;
                }
            }
            if (validate == true)
            {
                shipment.ConfirmOrNot = true;
                shipment.PickupOrNot = true;
                db.Entry(shipment).State = EntityState.Modified;
                foreach (Package package in shipment.Packages)
                {
                    db.Entry(package).State = EntityState.Modified;
                }

                tracking.Description = "Picked up";
                tracking.Location = Request.Form["TrackingPickupLocation"];
                tracking.Remark = Request.Form["PickupRemark"];
                tracking.Time = DateTime.Now;
                db.Trackings.Add(tracking);


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shipment);
        }



        public ActionResult GetRecipient(string RecipientAddressNickname)
        {
            if (string.IsNullOrEmpty(RecipientAddressNickname))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.Recipients.Single(hehe => hehe.Nickname == RecipientAddressNickname);
            Recipient data = query;
            data.ShippingAccount = null;
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPickupLocation(string PickupLocationNickname)
        {
            if (string.IsNullOrEmpty(PickupLocationNickname))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            ShippingAccount current_account = GetCurrentAccount();
            var query = db.PickupLocations.Single(hehe => hehe.Nickname == PickupLocationNickname && hehe.ShippingAccountId == current_account.ShippingAccountId);
            PickupLocation data = query;
            data.ShippingAccount = null;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPayers(string ShipmentPayer, string TaxPayer)
        {
            if (string.IsNullOrEmpty(ShipmentPayer) || string.IsNullOrEmpty(TaxPayer))
            {
                return Json(new List<string>(2), JsonRequestBehavior.AllowGet);
            }

            List<string> result = new List<string> { ShipmentPayer, TaxPayer };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReferenceNumber(string ReferenceNumber)
        {
            if (string.IsNullOrEmpty(ReferenceNumber))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            ShippingAccount current_account = GetCurrentAccount();
            var hehe = db.Shipments.Where(a => a.SenderShippingAccountID == current_account.ShippingAccountId).Select(a => a.ReferenceNumber);
            if (hehe.Contains(ReferenceNumber))
            {
                return Json(current_account.UserName, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

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
        //.....

    }
}
