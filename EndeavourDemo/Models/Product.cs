using System;
using System.Collections.Generic;

#nullable disable

namespace EndeavourDemo.Models
{
    public partial class Product
    {
        public Product()
        {
            PromotionRules = new HashSet<PromotionRule>();
            TrolleyItems = new HashSet<TrolleyItem>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual ICollection<PromotionRule> PromotionRules { get; set; }
        public virtual ICollection<TrolleyItem> TrolleyItems { get; set; }
    }
}
