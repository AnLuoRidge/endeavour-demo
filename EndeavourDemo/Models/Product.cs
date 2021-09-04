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
            Trolleys = new HashSet<Trolley>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public string Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual ICollection<PromotionRule> PromotionRules { get; set; }
        public virtual ICollection<Trolley> Trolleys { get; set; }
    }
}
