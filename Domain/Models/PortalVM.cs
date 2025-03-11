using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain.Models
{
    public class PortalVM
    {
        public List<ProductVM>? ProductVMs{ get; set; }
        public List<CategoryVM>? CategoryVMs{ get; set; }
    }
}
