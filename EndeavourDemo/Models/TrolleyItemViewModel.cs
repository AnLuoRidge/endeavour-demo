using System;
using System.Collections.Generic;

namespace EndeavourDemo.Models
{
    public class TrolleyItemViewModel
    {
        public int TrolleyItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Qty { get; set; }
        public decimal OriginalUnitPrice { get; set; }
        public decimal RealUnitPrice { get; set; }
        public decimal RealSubtotal { get; set; } // Original subtotal could be calculated.
        public List<PromotionRule> Promotions { get; set; }
    }
}
