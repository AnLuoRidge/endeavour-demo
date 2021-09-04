using System;
using System.Collections.Generic;

#nullable disable

namespace EndeavourDemo.Models
{
    public partial class Trolley
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public virtual Product Product { get; set; }
    }
}
