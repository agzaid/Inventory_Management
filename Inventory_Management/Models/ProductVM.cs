﻿namespace Inventory_Management.Models
{
    public class ProductVM
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public int QuantityAvailable { get; set; }
    }
}