using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SinExWebApp20328800.Models
{
    public class Penalty
    {
        public virtual int PenaltyID { get; set; }
        [Display(Name = "Penalty Charge")]
        public virtual decimal PenaltyCharge { get; set; }

    }
}