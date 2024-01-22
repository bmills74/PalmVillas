using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PalmVillas.Domain
{
    public class Booking
    {
        public string UserId { get; set; } = null!;

        public long BookingId { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; } = null!;

        [Display(Name = "End date")]
        public string EndDate { get; set; } = null!;

        [Display(Name = "Total cost")]
        public long Price { get; set; }

        public long? VillaId { get; set; }

        public virtual User User { get; set; } = null!;

        public Villa? Villa { get; set; }
    }
}


