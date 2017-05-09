using System;
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
        [Display(Name ="Estimated total shipment cost")]
        public virtual decimal EstimatedShipmentTotalAmount { get; set; }
        [Display(Name = "Actual total shipment cost")]
        public virtual decimal ShipmentTotalAmount { get; set; }

        [Required]
        [Display(Name = "Confirm")]
        public virtual bool ConfirmOrNot { get; set; }
        [Required]
        [Display(Name = "Pickup")]
        public virtual bool PickupOrNot { get; set; }
        [Required]
        [Display(Name = "Cancelled")]
        public virtual bool CancelledOrNot { get; set; }
        [Required]
        [Display(Name = "Terminated")]
        public virtual bool DeliveredOrNot { get; set; }
        [Required]
        [Display(Name ="Notify sender")]
        public virtual bool NotifySenderOrNot { get; set; }
        [Required]
        [Display(Name = "Notify recipient")]
        public virtual bool NotifyRecipientOrNot { get; set; }

        [Display(Name = "Pickup Type")]
        public virtual PickupType? PickupType { get; set; }
        [Display(Name = "pickup Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",  ApplyFormatInEditMode = true)]
        public virtual DateTime? PickupDate { get; set; }
        [Display(Name = "delivered Date")]
        public virtual DateTime? DeliveredDate { get; set; }


        // public virtual int RecipientID { get; set; }
        [Display(Name = "Recipient Nickname")]
        public virtual string RecipientAddressNickname { get; set; }
        [Required]
        [Display(Name = "Recipient Name")]
        public virtual string RecipientFullName { get; set; }
        [Display(Name = "Recipient Company Name")]
        public virtual string RecipientCompanyName { get; set; }
        [Display(Name = "Recipient Department Name")]
        public virtual string RecipientDepartmentName { get; set; }
        [Display(Name = "Recipient Delivery Building")]
        public virtual string RecipientDeliveryBuilding { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery Street")]
        public virtual string RecipientDeliveryStreet { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery City")]
        public virtual string RecipientDeliveryCity { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery Province")]
        public virtual string RecipientDeliveryProvince { get; set; }
        [Display(Name = "Recipient Delivery Postcode")]
        public virtual string RecipientDeliveryPostcode { get; set; }
        [Required]
        [Display(Name = "Recipient Phone")]
        [StringLength(14, MinimumLength = 8)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must be numeric")]
        public virtual string RecipientPhoneNumber { get; set; }
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Incorrect email address")]
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
        [InverseProperty("Shipment")]
        public virtual ICollection<Package> Packages { get; set; }
        [InverseProperty("Shipment")]
        public virtual ICollection<Tracking> Trackings { get; set; }
        [InverseProperty("Shipment")]
        public virtual ICollection<Payment> Payments { get; set; }


    }
}