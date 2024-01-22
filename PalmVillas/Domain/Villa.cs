using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PalmVillas.Domain
{
    public class Villa
    {
        public long Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string? Name { get; set; }

        public string? Images { get; set; }

        [Range(10, 10000)]
        [Required]
        public long? Price { get; set; }

        [Range(1, 10)]
        [Required]
        public long? Rooms { get; set; }

        [StringLength(600, MinimumLength = 3)]
        [Required]
        public string? Description { get; set; }


    }
}


