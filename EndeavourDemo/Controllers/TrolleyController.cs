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

        [HttpGet]
        public async Task<IActionResult> GetTrolley()
        {
            var rawTrolleyItems = _ctx.TrolleyItems
                .Include(ti => ti.Product)
                .ThenInclude(p => p.PromotionRules)
                .Select(ti => new TrolleyItemWithCompletePromotionModel
                {
                    TrolleyItemId = ti.TrolleyItemId,
                    ProductId = ti.ProductId,
                    ProductName = ti.Product.Name,
                    Qty = ti.Qty,
                    OriginalUnitPrice = ti.Product.UnitPrice,
                    RealUnitPrice = ti.Product.UnitPrice,
                    OriginalSubtotal = 0,
                    PromotionRules = ti.Product.PromotionRules.Where(r => r.IsActive == true),
                    Promotions = ti.Product.PromotionRules.Select(pr => new PromotionRuleViewModel
                    {
                        PromotionRuleId = pr.PromotionRuleId,
                        Name = pr.Name,
                        Definition = pr.Definition
                    })
                }).ToList(); // Would be simpler with AutoMapper

            // Map to the model for return. It trims the promotion detail.
            IEnumerable<TrolleyItemViewModel> trolleyItems = rawTrolleyItems;

            // Apply promotions
            // 1. single item
            // (2. cross item, bundle)
            // 3. whole trolley

            // 1 Single item promotion
            for (int i = 0; i < rawTrolleyItems.Count(); i++)
            {
                var rawItem = rawTrolleyItems.ElementAt(i);
                var item = trolleyItems.ElementAt(i);
                item.OriginalSubtotal = item.OriginalUnitPrice * item.Qty;

                // Assume only one promotion for one product for now.
                // TODO check product_code, expiry data, etc matched,
                var promotionRule = rawItem.PromotionRules.FirstOrDefault();
                if (promotionRule is not null)
                {
                    // Create a data table for calculation.
                    DataTable dt = new();
                    var unitPriceCal = promotionRule.UnitPriceExpression.Replace("unit_price", item.OriginalUnitPrice.ToString());
                    item.RealUnitPrice = (decimal)dt.Compute(unitPriceCal, "");

                    var subtotalCal = promotionRule.SubtotalExpression.Replace("unit_price", item.OriginalUnitPrice.ToString());
                    subtotalCal = subtotalCal.Replace("qty", item.Qty.ToString());
                    item.RealSubtotal = (decimal)dt.Compute(subtotalCal, ""); ;
                }
                else
                {
                    item.RealSubtotal = item.OriginalUnitPrice * item.Qty;
                }
            }

            // 3 Trolly promotion
            TrolleyViewModel trolley = new();
            trolley.Count = trolleyItems.Count();
            trolley.Items = trolleyItems.ToList();
            trolley.OriginalTotal = trolleyItems.Sum(ti => ti.OriginalSubtotal);
            trolley.RealTotal = trolleyItems.Sum(ti => ti.RealSubtotal); // The real total before applying trolley level promotion.
            // Assume only one promotion for the trolley now.
            var trolleyPromotionRules = await _ctx.PromotionRules.FirstOrDefaultAsync(r => r.Scope == 3 && r.IsActive == true); // 1 product 2 cross-product 3 trolley. The same comment in the database.
            if (trolleyPromotionRules is not null)
            {
                // Create a data table for calculation.
                DataTable dt = new();
                var cal = trolleyPromotionRules.SubtotalExpression.Replace("total", trolley.RealTotal.ToString());
                trolley.RealTotal = (decimal)dt.Compute(cal, "");
            }

            return Ok(trolley);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromQuery] int productId, [FromQuery] int qty)
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

        [HttpDelete("remove/{trolleyItemId}")]
        public async Task<IActionResult> Remove(int trolleyItemId)
        {
            var trolleyItem = _ctx.TrolleyItems.FirstOrDefault(ti => ti.TrolleyItemId == trolleyItemId);
            if (trolleyItem is not null)
            {
                _ctx.TrolleyItems.Remove(trolleyItem);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] int trolleyItemId, [FromQuery] int qty)
        {
            if (qty < 0)
            {
                return BadRequest("Invalid quantity");
            }

            var trolleyItem = _ctx.TrolleyItems.FirstOrDefault(ti => ti.TrolleyItemId == trolleyItemId);
            if (trolleyItem is not null)
            {
                if (qty == 0)
                {
                    await Remove(trolleyItemId);
                }
                else
                {
                    trolleyItem.Qty = qty;
                    _ctx.Entry(trolleyItem).Property(t => t.DateCreated).IsModified = false;
                    await _ctx.SaveChangesAsync();
                }
                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return NotFound();
            }

        }
    }
}
