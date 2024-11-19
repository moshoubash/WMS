using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/chart-data")]
    [ApiController]
    public class ChartDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChartDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetChartData()
        {
            var inventoryData = _context.Inventories
                .Include(i => i.Product)
                .GroupBy(i => i.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalProductsInventories = g.Sum(i => i.Quantity)
                })
                .ToList();

            var labels = inventoryData.Select(s => s.ProductName).ToArray();
            var values = inventoryData.Select(s => s.TotalProductsInventories).ToArray();

            return Ok(new { labels, values });
        }

        [HttpGet]
        [Route("Orders")]
        public IActionResult GetOrderChartData()
        {
            var orderData = _context.Orders
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalOrders = g.Count()
                })
                .OrderBy(o => o.Month)
                .ToList();

            var monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            var labels = orderData.Select(o => monthNames[o.Month - 1]).ToArray();
            var values = orderData.Select(o => o.TotalOrders).ToArray();

            return Ok(new { labels, values });
        }

    }

}
