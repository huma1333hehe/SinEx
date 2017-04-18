using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Recipient")]
    public class Recipient
    {
        public virtual int RecipientID { get; set; }
        public virtual int ShippingAccountId { get; set; }

        [Required]
        [StringLength(maximumLength: 70, MinimumLength = 1, ErrorMessage = "Length requirements: (0,70]")]
        [Display(Name = "Recipient Name")]
        public virtual string FullName { get; set; }

        [StringLength(maximumLength: 40, MinimumLength = 1, ErrorMessage = "Length requirements: (0,40]")]
        [Display(Name = "Company Name")]
        public virtual string CompanyName { get; set; }

        [StringLength(maximumLength: 30, MinimumLength = 1, ErrorMessage = "Length requirements: (0,30]")]
        [Display(Name = "Department Name")]
        public virtual string DepartmentName { get; set; }

        [Display(Name = "Recipient Delivery Building")]
        public virtual string DeliveryBuilding { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery Street")]
        public virtual string DeliveryStreet { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery City")]
        public virtual string DeliveryCity { get; set; }
        [Required]
        [Display(Name = "Recipient Delivery Province")]
        public virtual string DeliveryProvince { get; set; }
        [Display(Name = "Recipient Delivery Postcode")]
        public virtual string DeliveryPostcode { get; set; }

        [Required]
        [StringLength(maximumLength: 14, MinimumLength = 8, ErrorMessage = "Phone number should contain at least 8 digits, at most 14 digits")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone Number must be numeric.")]
        [Display(Name = "Phone Number")]
        public virtual string PhoneNumber { get; set; }

        [Required]
        [StringLength(maximumLength: 30, ErrorMessage = "Email address can not exceed 30 characters")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 1, ErrorMessage = "Length requirements: (0,10]")]
        [Display(Name = "Nick Name")]
        public virtual string Nickname { get; set; }

        [ForeignKey("ShippingAccountId")]
        public virtual ShippingAccount ShippingAccount { get; set; }
    }
}