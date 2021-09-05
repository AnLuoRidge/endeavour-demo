using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using EndeavourDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EndeavourDemo.Controllers
{
    [ApiController]
    [Route("trolley")]
    public class TrolleyController : ControllerBase
    {
        private EndeavourContext _ctx;

        public TrolleyController(EndeavourContext context)
        {
            _ctx = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToTrolley([FromQuery] int productId, [FromQuery] int qty)
        {
            if (qty <= 0)
            {
                return BadRequest("Invalid quantity");
            }
            var trolleyItem = _ctx.TrolleyItems.FirstOrDefault(ti => ti.ProductId == productId);
            if (trolleyItem is not null)
            {
                trolleyItem.Qty++;
                _ctx.SaveChanges();
                return Ok(trolleyItem.TrolleyItemId);
            }
            else
            {
                var product = await _ctx.Products.FindAsync(productId);
                if (product is not null)
                {
                    var newItem = new TrolleyItem
                    {
                        ProductId = product.ProductId,
                        Qty = qty
                    };
                    _ctx.TrolleyItems.Add(newItem);
                    _ctx.SaveChanges();
                    return Ok(newItem.TrolleyItemId);
                }
                else
                {
                    return NotFound();
                }
            }
        }
    }
}
