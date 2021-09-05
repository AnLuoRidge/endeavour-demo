using System;
using System.Linq;
using System.Threading.Tasks;
using EndeavourDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace EndeavourDemo.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        private EndeavourContext _ctx;

        public ProductController(EndeavourContext context)
        {
            _ctx = context;
        }

        [HttpGet("list")]
        public IActionResult getAllProducts()
        {
            var products = _ctx.Products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                UnitPrice = p.UnitPrice
            });
            return Ok(products);
        }
    }
}
