using System;
using System.Collections.Generic;

#nullable disable

namespace EndeavourDemo.Models
{
    public partial class PromotionRule
    {
        public int PromotionRuleId { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public int? ProductId { get; set; }
        public int Priority { get; set; }
        public string Type { get; set; }
        public string UnitPriceExpression { get; set; }
        public string SubtotalExpression { get; set; }
        public int Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual Product Product { get; set; }
    }
}
