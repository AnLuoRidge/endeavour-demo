using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EndeavourDemo.Models;

namespace EndeavourDemo.Services
{
    public class TrolleyService
    {
        public void applyIndividualPromotion(IEnumerable<TrolleyItemViewModel> trolleyItems, IEnumerable<PromotionRule> promotionRules)
        {
            for (int i = 0; i < trolleyItems.Count(); i++)
            {
                var promotionRule = promotionRules.ElementAt(i);
                var item = trolleyItems.ElementAt(i);
                item.OriginalSubtotal = item.OriginalUnitPrice * item.Qty;

                // Assume only one promotion for one product for now.
                // TODO check product_code, expiry date, etc..
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
        }
    }
}
