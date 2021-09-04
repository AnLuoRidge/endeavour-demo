using System;
using System.Collections.Generic;

#nullable disable

namespace EndeavourDemo.Models
{
    public partial class TrolleyViewModel
    {
        public int Count { get; set; }
        public decimal RealTotal { get; set; }
        public decimal OriginalTotal { get; set; }
        public List<TrolleyItemViewModel> Items { get; set; }
    }
}
