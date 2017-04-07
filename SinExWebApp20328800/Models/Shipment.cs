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
    [Table("Shipment")]
    public class Shipment
    {
        [Key]
        public virtual int WaybillId { get; set; }
        public virtual string ReferenceNumber { get; set; }

        public virtual int NumberOfPackages { get; set; }

        public virtual ShipmentPayer ShipmentPayer { get; set; }
        public virtual TaxPayer TaxPayer { get; set; }

        public virtual decimal? Duty { get; set; }
        public virtual decimal? Tax { get; set; }

        public virtual bool ConfirmOrNot { get; set; }
        public virtual bool PickupOrNot { get; set; }
        public virtual DateTime PickupDate { get; set; }

        public virtual int RecipientID { get; set; }
        public virtual int ServiceTypeID { get; set; }
        public virtual int PickupLocationID { get; set; }
        //[Display(Name = "Sender's Shipping Account")]
        public virtual int SenderShippingAccountID { get; set; }
        public virtual int RecipientShippingAccountID { get; set; }


        public virtual Recipient Recipient { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual PickupLocation PickupLocation { get; set; }
        [InverseProperty("Shipments")]
        [ForeignKey("SenderShippingAccountID")]
        public virtual ShippingAccount SenderShippingAccount { get; set; }
        [ForeignKey("RecipientShippingAccountID")]
        public virtual ShippingAccount RecipientShippingAccount { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<Tracking> Trackings { get; set; }

    }
}