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
        [Display(Name = "Package ID")]
        public virtual int PackageID { get; set; }
        [Display(Name = "Waybill ID")]
        public virtual int WaybillId { get; set; }
        [Display(Name = "Package Type")]
        public virtual int PackageTypeID { get; set; }
        [Display(Name = "Package Type Size")]
        public virtual int PackageTypeSizeID { get; set; }
        [Display(Name = "Description")]
        public virtual string Description { get; set; }
        [Display(Name = "Value")]
        [Range(typeof(decimal), "0.01", "99999999", ErrorMessage = "Invalid range")]
        [RegularExpression(@"^[0-9]*([.][0-9]{1,2})?$", ErrorMessage = "Please input no more than decimal places only")]
        public virtual decimal Value { get; set; }
        [Display(Name = "Currency")]
        public virtual string CurrencyCode { get; set; }
        [Required]
        [Range(typeof(decimal), "0.0", "99999999", ErrorMessage = "Invalid range")]
        [Display(Name = "Declared Weight")]
        public virtual decimal DeclaredWeight { get; set; }
        [Range(typeof(decimal), "0.0", "99999999", ErrorMessage = "Invalid range")]
        [Display(Name = "Actual Weight")]
        public virtual decimal? ActualWeight { get; set; }
        [Display(Name = "Declared Fee")]
        public virtual decimal DeclaredFee { get; set; }
        [Display(Name = "Actual Fee")]
        public virtual decimal? ActualFee { get; set; }
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
