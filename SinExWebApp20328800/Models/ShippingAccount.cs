using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SinExWebApp20328800.Validators;

namespace SinExWebApp20328800.Models
{
    [Table("ShippingAccount")]
    public abstract class ShippingAccount
    {
        [Required]
        public virtual int ShippingAccountId { get; set; }
        [StringLength(10)]
        public virtual string UserName { get; set; }
        [Required]
        [StringLength(14, MinimumLength = 8)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number must be numeric")]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }
        [Required]
        [StringLength(30)]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Incorrect email address")]
        [Display(Name = "Email Address")]
        public virtual string EmailAddress { get; set; }
        [StringLength(50)]
        [Display(Name = "Building")]
        public virtual string BuildingInformation { get; set; }
        [Required]
        [StringLength(35)]
        [Display(Name = "Street")]
        public virtual string StreetInformation { get; set; }
        [Required]
        [StringLength(25)]
        [Display(Name = "City")]
        public virtual string City { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2)]
        [Display(Name = "Province Code")]
        public virtual string ProvinceCode { get; set; }
        [StringLength(6, MinimumLength = 5)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Postal code must be numeric")]
        [Display(Name = "Postal Code")]
        public virtual string PostalCode { get; set; }
        [Required]
        //[ItemInList(new string[6]{"American Express","Diners Club","Discover","masterCard","UnionPay","Visa"})]
        [Display(Name = "Type")]
        public virtual string Type { get; set; }
        [Required]
        [StringLength(19, MinimumLength = 14)]
        [Display(Name = "Number")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Credit card number must be numeric")]
        public virtual string Number { get; set; }
        [Required]
        [StringLength(4, MinimumLength = 3)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Security number must be numeric")]
        [Display(Name = "Security Number")]
        public virtual string SecurityNumber { get; set; }
        [Required]
        [StringLength(75)]
        [Display(Name = "Cardholder Name")]
        public virtual string CardholderName { get; set; }
        [Required]
        [StringLength(2, ErrorMessage = "Please enter valid month number (1-12)")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Expiry month must be numeric")]
        [Display(Name = "Expiry Month")]
        public virtual string ExpiryMonth { get; set; }
        [Required]
        [StringLength(4)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Expiry year not valid")]
        [Display(Name = "Expiry year")]
        public virtual string ExpiryYear { get; set; }

        [InverseProperty("SenderShippingAccount")]
        public virtual ICollection<Shipment> Shipments { get; set; }
    }
}
