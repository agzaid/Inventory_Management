﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Brand : BaseEntity
    {
        [MaxLength(100)]
        public string? BrandName { get; set; }
        [MaxLength(100)]
        public string? BrandNameAr { get; set; }
        [MaxLength(800)]
        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }
        public ICollection<Image>? Images { get; set; } // Navigation property

    }
}
