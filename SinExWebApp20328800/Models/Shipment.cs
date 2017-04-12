﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    public enum ShipmentPayer
    {
        Sender,
        Recipient,
    }
    public enum TaxPayer
    {
        Sender,
        Recipient,
    }
    public enum PickupType
    {
        Immediate,
        Prearranged,
    }
    [Table("Shipment")]
    public class Shipment
    {
        [Key]
        [Display(Name = "Waybill ID")]
        public virtual int WaybillId { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 1)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Reference number must be numeric")]
        [Display(Name = "Ref No.")]
        public virtual string ReferenceNumber { get; set; }

        [Required]
        [Display(Name = "Origin")]
        public virtual string Origin { get; set; }
        [Required]
        [Display(Name = "Destination")]
        public virtual string Destination { get; set; }

        [Required]
        [Display(Name = "No. of Packages")]
        public virtual int NumberOfPackages { get; set; }

        [Required]
        [Display(Name = "Shipment Payer")]
        public virtual ShipmentPayer ShipmentPayer { get; set; }
        [Required]
        [Display(Name = "Tax Payer")]
        public virtual TaxPayer TaxPayer { get; set; }

        public virtual decimal? Duty { get; set; }
        public virtual decimal? Tax { get; set; }

        [Required]
        [Display(Name = "Confirm")]
        public virtual bool ConfirmOrNot { get; set; }
        [Required]
        [Display(Name = "Pickup")]
        public virtual bool PickupOrNot { get; set; }

        [Required]
        [Display(Name = "Pickup Type")]
        public virtual PickupType PickupType { get; set; }
        [Required]
        [Display(Name = "pickup Date")]
        public virtual DateTime PickupDate { get; set; }


        // public virtual int RecipientID { get; set; }
        [Display(Name = "Recipient Nickname")]
        public virtual string RecipientAddressNickname { get; set; }
        [Required]
        [Display(Name = "Recipient Name")]
        public virtual string RecipientFullName { get; set; }
        [Required]
        [Display(Name = "Recipient Company Name")]
        public virtual string RecipientCompanyName { get; set; }
        [Display(Name = "Recipient Department Name")]
        public virtual string RecipientDepartmentName { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery Address")]
        public virtual string RecipientDeliveryAddress { get; set; }
        [Required]
        [Display(Name = "Recipient Phone")]
        public virtual string RecipientPhoneNumber { get; set; }
        [Required]
        [Display(Name = "Recipient Email")]
        public virtual string RecipientEmail { get; set; }
        [Required]
        [Display(Name = "Service Type")]
        public virtual int ServiceTypeID { get; set; }

        [Display(Name = "Pickup Location Nickname")]
        public virtual string PickupLocationNickname { get; set; }
        [Required]
        [Display(Name = "Pickup Location")]
        public virtual string PickupLocation { get; set; }

        //[Display(Name = "Sender's Shipping Account")]
        [Display(Name = "Sender Shipping Account")]
        public virtual int SenderShippingAccountID { get; set; }
        [Display(Name = "Recipient Shipping Account")]
        public virtual int RecipientShippingAccountID { get; set; }

        public virtual ServiceType ServiceType { get; set; }

        [InverseProperty("Shipments")]
        [ForeignKey("SenderShippingAccountID")]
        public virtual ShippingAccount SenderShippingAccount { get; set; }
        [ForeignKey("RecipientShippingAccountID")]
        public virtual ShippingAccount RecipientShippingAccount { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<Tracking> Trackings { get; set; }

    }
}