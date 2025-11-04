using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingFreight : BaseEntity
    {
        //Area > District 
        [MaxLength(300)]
        public string? ShippingArea { get; set; }
        [MaxLength(300)]
        public string? ShippingAreaAr { get; set; }
        //public string? District { get; set; }
        public decimal? Price { get; set; }

        public ICollection<District>? Districts { get; set; }

        // public ICollection<Product>? Products { get; set; }
        public string? DisplayShippingAreaName
        {
            get
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                return lang == "ar" ? ShippingAreaAr : ShippingArea;
            }
        }
    }
}
