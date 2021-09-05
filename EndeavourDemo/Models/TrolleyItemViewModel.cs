using System;
using System.Collections.Generic;

namespace EndeavourDemo.Models
{
    public class TrolleyItemViewModel
    {
        public int TrolleyItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public decimal OriginalUnitPrice { get; set; }
        public decimal RealUnitPrice { get; set; }
        public decimal OriginalSubtotal { get; set; }
        public decimal RealSubtotal { get; set; }
        public IEnumerable<PromotionRuleViewModel> Promotions { get; set; }
    }

    public class TrolleyItemWithCompletePromotionModel : TrolleyItemViewModel
    {
        public IEnumerable<PromotionRule> PromotionRules { get; set; }
    }
}
