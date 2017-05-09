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
using System.Net.Mail;
using System.Threading.Tasks;

namespace SinExWebApp20328800.Controllers
{

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
                Value = "0",
                Selected = false
            });
            shipmentSearch.Shipment.ShippingAccountId = ShippingAccountId == null ? 0 : (int)ShippingAccountId;
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
            else
            {
                // Add the condition to select a spefic shipping account if shipping account id is not null.
                if (ShippingAccountId != 0 && ShippingAccountId != null)
                {
                    // TODO: Construct the LINQ query to retrive only the shipments for the specified shipping account id.
                    shipmentQuery = shipmentQuery.Where(s => s.ShippingAccountId == ShippingAccountId);

                }
            }



            if ((StartingDate != null) && (EndingDate != null) && DateType != null)
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
            ShippingAccount current_account = GetCurrentAccount();
            var shipments = db.Shipments.Include(s => s.RecipientShippingAccount).Include(s => s.SenderShippingAccount).Include(s => s.ServiceType).
                Where(s => s.CancelledOrNot == false && s.ConfirmOrNot == false && s.PickupOrNot == false && s.NumberOfPackages > 0 && s.SenderShippingAccountID == current_account.ShippingAccountId);

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

            if (Request.Form["PickupType"] != null && Request.Form["PickupDate"] != null && ModelState.IsValid)
            {
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
            }
            else
            {
                ShippingAccount current_account = GetCurrentAccount();
                IEnumerable<PickupLocation> lala = db.PickupLocations.Select(g => g).Where(g => g.ShippingAccountId == current_account.ShippingAccountId);
                ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
                ViewBag.PickupTypeEmpty = Request.Form["PickupType"] == null ? true : false;
                ViewBag.PickupDateEmpty = Request.Form["PickupDate"] == null ? true : false;
                return View(shipments);
            }


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
            ViewBag.taxCurrency = shipment.TaxPayer == TaxPayer.Recipient ? db.Destinations.SingleOrDefault(d => d.City == shipment.RecipientShippingAccount.City).CurrencyCode : db.Destinations.SingleOrDefault(d => d.City == shipment.SenderShippingAccount.City).CurrencyCode;
            ViewBag.dutyandtaxpayercity = shipment.TaxPayer == TaxPayer.Recipient ? shipment.RecipientShippingAccount.City : shipment.SenderShippingAccount.City;
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
        public ActionResult Create([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,DeliveredOrNot,ShipmentTotalAmount,NotifyRecipientOrNot,NotifySenderOrNot")] Shipment shipment)
        {
            ShippingAccount current_account = GetCurrentAccount();

            shipment.SenderShippingAccountID = current_account.ShippingAccountId;
            shipment.SenderShippingAccount = current_account;
            shipment.ServiceType = db.ServiceTypes.Single(a => a.ServiceTypeID == shipment.ServiceTypeID);
            shipment.ConfirmOrNot = false;
            shipment.PickupOrNot = false;
            shipment.CancelledOrNot = false;
            shipment.Duty = null;
            shipment.Tax = null;
            shipment.PickupType = null;
            shipment.PickupDate = null;
            shipment.NumberOfPackages = 0;
            shipment.ShipmentTotalAmount = 0;
            shipment.EstimatedShipmentTotalAmount = 0;

            bool is_recipient_valid = false;
            if (!(shipment.TaxPayer == TaxPayer.Recipient || shipment.ShipmentPayer == ShipmentPayer.Recipient))
            {
                shipment.RecipientShippingAccountID = current_account.ShippingAccountId;
                shipment.RecipientShippingAccount = current_account;
                is_recipient_valid = true;
            }
            else
            {
                ShippingAccount recipient_account = db.ShippingAccounts.SingleOrDefault(s => s.ShippingAccountId == shipment.RecipientShippingAccountID);
                if (recipient_account != null && recipient_account != current_account)
                {
                    shipment.RecipientShippingAccountID = recipient_account.ShippingAccountId;
                    shipment.RecipientShippingAccount = recipient_account;
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
            ViewBag.RecipientShippingAccountID = shipment.RecipientShippingAccountID;
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
        public ActionResult Edit([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,DeliveredOrNot,ShipmentTotalAmount,NotifySenderOrNot,NotifyRecipientOrNot")] Shipment shipment)
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
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false && s.PickupOrNot == false);
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
            ViewBag.taxCurrency = shipment.TaxPayer == TaxPayer.Recipient ? db.Destinations.SingleOrDefault(d => d.City == shipment.RecipientShippingAccount.City).CurrencyCode : db.Destinations.SingleOrDefault(d => d.City == shipment.SenderShippingAccount.City).CurrencyCode;
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
            Shipment shipment = db.Shipments.SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false && s.PickupOrNot == false && s.ConfirmOrNot == false);
            ViewBag.taxCurrency = shipment.TaxPayer == TaxPayer.Recipient ? db.Destinations.SingleOrDefault(d => d.City == shipment.RecipientShippingAccount.City).CurrencyCode : db.Destinations.SingleOrDefault(d => d.City == shipment.SenderShippingAccount.City).CurrencyCode;
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
        public ActionResult Confirm([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,DeliveredOrNot,ShipmentTotalAmount,EstimatedShipmentTotalAmount,NotifyRecipientOrNot,NotifySenderOrNot")] Shipment shipment)
        {
            if (ModelState.IsValid && shipment.PickupType != null && shipment.PickupDate != null)
            {
                shipment.ConfirmOrNot = true;
                shipment.PickupOrNot = false;
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ShippingAccount current_account = GetCurrentAccount();
                IEnumerable<PickupLocation> lala = db.PickupLocations.Select(s => s).Where(s => s.ShippingAccountId == current_account.ShippingAccountId);
                ViewBag.PickupLocationNickname = new SelectList(lala, "Nickname", "Nickname");
                ViewBag.PickupTypeEmpty = shipment.PickupType == null ? true : false;
                ViewBag.PickupDateEmpty = shipment.PickupDate == null ? true : false;
                return View(shipment);
            }

        }

        // GET: Shipments/Pickup
        public ActionResult Pickupindex()
        {
            var shipments = db.Shipments.Include(s => s.RecipientShippingAccount).Include(s => s.SenderShippingAccount).Include(s => s.ServiceType).Where(s => s.CancelledOrNot == false && s.ConfirmOrNot == true && s.DeliveredOrNot == false && s.PickupOrNot == false);
            return View(shipments.ToList());
        }

        // GET: Shipments/Pickup/5

        public ActionResult Pickup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipment shipment = db.Shipments.Include(s => s.Packages).SingleOrDefault(s => s.WaybillId == id && s.CancelledOrNot == false && s.PickupOrNot == false && s.ConfirmOrNot == true);
            ViewBag.taxCurrency = shipment.TaxPayer == TaxPayer.Recipient ? db.Destinations.SingleOrDefault(d => d.City == shipment.RecipientShippingAccount.City).CurrencyCode : db.Destinations.SingleOrDefault(d => d.City == shipment.SenderShippingAccount.City).CurrencyCode;
            ViewBag.dutyandtaxpayercity = shipment.TaxPayer == TaxPayer.Recipient ? shipment.RecipientShippingAccount.City : shipment.SenderShippingAccount.City;
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
        public async Task<ActionResult> Pickup([Bind(Include = "WaybillId,ReferenceNumber,Origin,Destination,NumberOfPackages,ShipmentPayer,TaxPayer,Duty,Tax,ConfirmOrNot,PickupOrNot,CancelledOrNot,PickupType,PickupDate,RecipientaddressNickname,RecipientFullName,RecipientCompanyName,RecipientDepartmentName,RecipientDeliveryBuilding,RecipientDeliveryStreet,RecipientDeliveryCity,RecipientDeliveryProvince,RecipientDeliveryPostcode,RecipientPhoneNumber,RecipientEmail,ServiceTypeID,PickupLocationNickname,PickupLocation,SenderShippingAccountID,RecipientShippingAccountID,RecipientAddressNickname,DeliveredOrNot,ShipmentTotalAmount,EstimatedShipmentTotalAmount,NotifyRecipientOrNot,NotifySenderOrNot")] Shipment shipment)
        {

            db.Shipments.Attach(shipment);
            db.Entry(shipment).Collection(p => p.Packages).Load();
            bool validate = true;
            if (Request.Form["Duty"] == "" || Request.Form["Tax"] == "" || Request.Form["TrackingPickupLocation"] == "" || Request.Form["PickupRemark"] == "")
            {
                ViewBag.Incomplete = "Input Incomplete";
                validate = false;
            }

            foreach (Package package in shipment.Packages)
            {
                package.DeclaredWeight = Math.Round(package.DeclaredWeight, 1);
                string key = "package_" + package.PackageID.ToString();
                if (Request.Form[key] != "")
                {
                    decimal value;
                    if (Decimal.TryParse(Request.Form[key], out value))
                    {
                        package.ActualWeight = Math.Round(value, 1);
                        package.ActualFee = CalculatePackageFee(package);
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
                shipment.ShipmentTotalAmount = 0;
                foreach (Package Package in shipment.Packages)
                {
                    shipment.ShipmentTotalAmount += CalculatePackageFee(Package);
                }

                Tracking tracking = new Tracking();
                tracking.WaybillId = shipment.WaybillId;
                tracking.Description = "Picked up";
                tracking.Location = Request.Form["TrackingPickupLocation"];
                tracking.Remark = Request.Form["PickupRemark"];
                tracking.Time = DateTime.Now;
                db.Trackings.Add(tracking);

                Payment ShipmentPayment = new Payment();
                Payment DutyAndTaxPayment = new Payment();
                ShipmentPayment.AuthorizationCode = GenerateRandomNo().First();
                ShipmentPayment.WaybillID = shipment.WaybillId;
                ShipmentPayment.Shipment = shipment;
                ShipmentPayment.PaymentAmount = shipment.ShipmentTotalAmount;
                ShipmentPayment.PaymentDescription = "Shipment payment";

                DutyAndTaxPayment.AuthorizationCode = GenerateRandomNo().ElementAt(1);
                DutyAndTaxPayment.WaybillID = shipment.WaybillId;
                DutyAndTaxPayment.Shipment = shipment;
                DutyAndTaxPayment.PaymentAmount = (decimal)shipment.Tax + (decimal)shipment.Duty;
                DutyAndTaxPayment.PaymentDescription = "Duty and tax payment";


                shipment.SenderShippingAccount = db.ShippingAccounts.SingleOrDefault(a => a.ShippingAccountId == shipment.SenderShippingAccountID);
                shipment.RecipientShippingAccount = db.ShippingAccounts.SingleOrDefault(a => a.ShippingAccountId == shipment.RecipientShippingAccountID);
                if (shipment.ShipmentPayer == ShipmentPayer.Sender)
                {
                    Destination hehe = db.Destinations.Single(a => a.City == shipment.SenderShippingAccount.City);
                    ShipmentPayment.PayerCharacter = "Sender";
                    ShipmentPayment.CurrencyCode = hehe.CurrencyCode;
                    ShipmentPayment.UserName = shipment.SenderShippingAccount.UserName;
                }
                else
                {
                    Destination hehe = db.Destinations.Single(a => a.City == shipment.RecipientShippingAccount.City);
                    ShipmentPayment.PayerCharacter = "Recipient";
                    ShipmentPayment.CurrencyCode = hehe.CurrencyCode;
                    ShipmentPayment.UserName = shipment.RecipientShippingAccount.UserName;
                }

                if (shipment.TaxPayer == TaxPayer.Sender)
                {
                    Destination hehe = db.Destinations.Single(a => a.City == shipment.SenderShippingAccount.City);
                    DutyAndTaxPayment.PayerCharacter = "Sender";
                    DutyAndTaxPayment.CurrencyCode = hehe.CurrencyCode;
                    DutyAndTaxPayment.UserName = shipment.SenderShippingAccount.UserName;
                }
                else
                {
                    Destination hehe = db.Destinations.Single(a => a.City == shipment.RecipientShippingAccount.City);
                    DutyAndTaxPayment.PayerCharacter = "Recipient";
                    DutyAndTaxPayment.CurrencyCode = hehe.CurrencyCode;
                    DutyAndTaxPayment.UserName = shipment.RecipientShippingAccount.UserName;
                }
                db.Payments.Add(ShipmentPayment);
                db.Payments.Add(DutyAndTaxPayment);


                db.SaveChanges();



                shipment.Payments.Add(ShipmentPayment);
                shipment.Payments.Add(DutyAndTaxPayment);
                shipment.Trackings.Add(tracking);

                db.Entry(shipment).State = EntityState.Modified;
                foreach (Package package in shipment.Packages)
                {
                    db.Entry(package).State = EntityState.Modified;
                }


                //send email to recipient 

                if (shipment.NotifyRecipientOrNot)
                {
                    var body_pickup = "<p>Dear user {0}: </p><p>The shipment with the following details has been picked up.It's now on tis way to meet you in {1}</p><p>Shipment waybill Id: {2}</p><p>Pick up remark: {3}</p><p>Sender name: {4}</p><p>Sender address: {5}</p><p>Pick up date: {6}</p>";
                    var message_pickup = new MailMessage();
                    string Username = shipment.RecipientShippingAccountID == shipment.SenderShippingAccountID ? shipment.RecipientFullName : shipment.RecipientShippingAccount.UserName;
                    string RecipientCity = shipment.RecipientShippingAccountID == shipment.SenderShippingAccountID ? shipment.RecipientDeliveryCity : shipment.RecipientShippingAccount.City;
                    string WaybillId = shipment.WaybillId.ToString("000000000000");
                    string Notification = tracking.Remark;
                    string SenderName = "";
                    if (shipment.SenderShippingAccount is PersonalShippingAccount)
                    {
                        PersonalShippingAccount b = (PersonalShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == shipment.SenderShippingAccountID);
                        SenderName = b.FirstName + b.LastName;
                    }
                    else
                    {
                        BusinessShippingAccount b = (BusinessShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == shipment.SenderShippingAccountID);
                        SenderName = b.ContactPersonName;
                    }
                    string SenderAddress = shipment.SenderShippingAccount.ProvinceCode + ", " + shipment.SenderShippingAccount.City + ", " + shipment.SenderShippingAccount.StreetInformation + ", " + shipment.SenderShippingAccount.BuildingInformation;
                    string PickupDate = ((DateTime)(shipment.PickupDate)).ToString("yyyy/MM/dd HH:mm");
                    message_pickup.To.Add(new MailAddress(shipment.RecipientShippingAccountID == shipment.SenderShippingAccountID ? shipment.RecipientEmail : shipment.RecipientShippingAccount.EmailAddress));
                    message_pickup.Subject = "Pick up Notification Email";
                    message_pickup.Body = String.Format(body_pickup, Username, RecipientCity, WaybillId, Notification, SenderName, SenderAddress, PickupDate);
                    message_pickup.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message_pickup);
                    }
                }





                foreach (Payment lala in shipment.Payments)
                {
                    var body = "<p>Dear user {0}: </p><p>You have just paid for a shipment. Here are the details.</p><p>Shipping account number: {1}</p><p>Shipment waybill Id: {2}</p><p>Ship(pickup) date: {3}</p><p>Service type: {4}</p><p>Sender's reference number: {5}</p><p>Sender full name: {6}</p><p>Sender mailing address: {7}</p><p>Recipient full name: {8}</p><p>Recipient delivery address: {9}</p><p>Credit card type: {10}</p><p>Credit card number(last four digits only): {11}</p><p>Authorization code: {12}</p><p>Total amount payable: {13} {14}</p><p>Payment type: {15}</p><p>The detailed packages information of this shipment can be found below: </p>";
                    var message = new MailMessage();
                    ShippingAccount lala_account = db.ShippingAccounts.Single(a => a.UserName == lala.UserName);
                    Shipment lala_shipment = db.Shipments.Single(a => a.WaybillId == lala.WaybillID);
                    string ShippingAccountUsername = lala_account.UserName;
                    string ShippingAccountNumber = lala_account.ShippingAccountId.ToString("0000000000000");
                    string ShipmentWaybillID = lala.WaybillID.ToString("000000000000");
                    string ShipDate = ((DateTime)(lala_shipment.PickupDate)).ToString("yyyy/MM/dd HH:mm");
                    string ServiceType = db.ServiceTypes.Single(a => a.ServiceTypeID == lala_shipment.ServiceTypeID).Type;
                    string SenderReferenceNumber = lala_shipment.ReferenceNumber;
                    string SenderFullName = "";
                    if (lala_shipment.SenderShippingAccount is PersonalShippingAccount)
                    {
                        PersonalShippingAccount temp = (PersonalShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == lala_shipment.SenderShippingAccountID);
                        SenderFullName = temp.FirstName + temp.LastName;
                    }
                    else
                    {
                        BusinessShippingAccount temp = (BusinessShippingAccount)db.ShippingAccounts.Single(a => a.ShippingAccountId == lala_shipment.SenderShippingAccountID);
                        SenderFullName = temp.ContactPersonName;
                    }
                    string SenderMailingAddress = lala_shipment.SenderShippingAccount.ProvinceCode + ", " + lala_shipment.SenderShippingAccount.City + ", " + lala_shipment.SenderShippingAccount.StreetInformation + ", " + lala_shipment.SenderShippingAccount.BuildingInformation;
                    string RecipientFullName = lala_shipment.RecipientFullName;
                    string RecipientDeliveryAddress = lala_shipment.RecipientDeliveryProvince + ", " + lala_shipment.RecipientDeliveryCity + ", " + lala_shipment.RecipientDeliveryStreet + ", " + lala_shipment.RecipientDeliveryBuilding;
                    string CreditCardType = lala_account.Type;
                    string CreditCardNumber = lala_account.Number.Substring(lala_account.Number.Length - 4);
                    string AuthorizationCode = lala.AuthorizationCode;
                    string CurrencyCode = lala.CurrencyCode;
                    string TotalAmountPayable = lala.PaymentAmount.ToString("0.00");
                    string PaymentType = lala.PaymentDescription;

                    var body_package = "";
                    int index = 1;
                    foreach (Package haha in lala_shipment.Packages)
                    {
                        string PackageType = haha.PackageType.Type;
                        string ActualWeight = haha.ActualWeight.ToString();
                        string Cost = ((decimal)(CalculatePackageFee(haha) * db.Currencies.Single(a => a.CurrencyCode == lala.CurrencyCode).ExchangeRate)).ToString("0.00");
                        body_package += ("<p> Package No. " + index.ToString() + "</p>" + "<p>Package type: " + PackageType + "</p>" + "<p>Actual weight: " + ActualWeight + " kg</p>" + "<p>Cost: " + CurrencyCode + " " + Cost + "</p>");
                        index++;
                    }
                    message.To.Add(new MailAddress(db.ShippingAccounts.Single(a => a.UserName == lala.UserName).EmailAddress));
                    message.Subject = "Payment Notification and Invoice";
                    message.Body = String.Format(body, ShippingAccountUsername, ShippingAccountNumber, ShipmentWaybillID, ShipDate, ServiceType, SenderReferenceNumber, SenderFullName, SenderMailingAddress, RecipientFullName, RecipientDeliveryAddress, CreditCardType, CreditCardNumber, AuthorizationCode, CurrencyCode, TotalAmountPayable, PaymentType) + body_package;
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        await smtp.SendMailAsync(message);
                    }
                }
                db.SaveChanges();
                return View("PickupSuccess");
            }
            return View(shipment);
        }

        //Generate RandomNo
        private List<string> GenerateRandomNo()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            string a = _rdm.Next(_min, _max).ToString("0000");
            string b = _rdm.Next(_min, _max).ToString("0000");
            List<string> c = new List<string>(2);
            c.Add(a);
            c.Add(b);
            return c;
        }

        //Calculate package fee from actual weight in RMB
        private decimal CalculatePackageFee(Package Package)
        {
            ServicePackageFee hehe = db.ServicePackageFees.SingleOrDefault(a => a.PackageTypeID == Package.PackageTypeID && a.ServiceTypeID == Package.Shipment.ServiceTypeID);
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
                    price = Package.ActualWeight * hehe.Fee > hehe.MinimumFee ? (decimal)Package.ActualWeight * hehe.Fee : hehe.MinimumFee;
                    int limit = 0;
                    string limitString = Package.PackageTypeSize.WeightLimit;
                    bool convertResult = Int32.TryParse(limitString.Substring(0, limitString.Length - 2), out limit);
                    if (limit != 0 && convertResult && Package.ActualWeight > (decimal)limit)
                    {
                        price += db.Penalties.FirstOrDefault().PenaltyCharge;
                    }

                    break;
                //Tube or Custmoer
                case 3:
                case 5:
                    price = Package.ActualWeight * hehe.Fee > hehe.MinimumFee ? (decimal)Package.ActualWeight * hehe.Fee : hehe.MinimumFee;
                    break;
            }
            return price;
        }

        public ActionResult GetRecipient(string RecipientAddressNickname)
        {
            if (string.IsNullOrEmpty(RecipientAddressNickname))
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            ShippingAccount account = GetCurrentAccount();
            var query = db.Recipients.Single(hehe => hehe.Nickname == RecipientAddressNickname && hehe.ShippingAccountId == account.ShippingAccountId);
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

        public ActionResult GetCurrentDateTime(string PickupType)
        {
            if (string.IsNullOrEmpty(PickupType))
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            DateTime date = DateTime.Now;
            if (PickupType == "1")
            {
                date = DateTime.Now.AddDays(5.0);
            }
            return Json(date, JsonRequestBehavior.AllowGet);

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
