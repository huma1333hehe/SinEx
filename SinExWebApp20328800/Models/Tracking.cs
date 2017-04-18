using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Tracking")]
    public class Tracking
    {
        public virtual int TrackingID { get; set; }

        [Required]
        public virtual int WaybillId { get; set; }

        [Required]
        public virtual DateTime Time { get; set; }

        [Required]
        public virtual string Description { get; set; }

        [Required]
        public virtual string Location { get; set; }

        [Required]
        public virtual string Remark { get; set; }

        [ForeignKey("WaybillId")]
        public virtual Shipment Shipment { get; set; }
    }
}
