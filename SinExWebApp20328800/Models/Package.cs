using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Package")]
    public class Package
    {
        public virtual int PackageID { get; set; }
        public virtual int WaybillId { get; set; }
        public virtual int PackageTypeID { get; set; }
        public virtual int PackageTypeSizeID { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Value { get; set; }
        public virtual string CurrencyCode { get; set; }
        [Required]
        public virtual decimal DeclaredWeight { get; set; }
        public virtual decimal? ActualWeight { get; set; }

        [ForeignKey("PackageTypeID")]
        public virtual PackageType PackageType { get; set; }
        [ForeignKey("PackageTypeSizeID")]
        public virtual PackageTypeSize PackageTypeSize { get; set; }
        [ForeignKey("CurrencyCode")]
        public virtual Currency Currency { get; set; }
        [ForeignKey("WaybillId")]
        public virtual Shipment Shipment { get; set; }

    }
}
