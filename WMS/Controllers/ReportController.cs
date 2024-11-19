using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;
using WMS.Services;

namespace WMS.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly GenerateOrderReport _orderReportService;

        public ReportController(ApplicationDbContext dbContext, GenerateOrderReport orderReportService)
        {
            _dbContext = dbContext;
            _orderReportService = orderReportService;

        }
        [HttpGet]
        [Route("/report/generate/{id}")]
        public IActionResult DownloadOrderReport(int id)
        {
            // Retrieve the order from your database using the provided id
            var order = _dbContext.Orders.Include(o => o.OrderItems)
                                         .Include(o => o.Address)
                                         .Include(o => o.Customer)
                                         .ThenInclude(c => c.ContactInfo)
                                         .FirstOrDefault(o => o.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Generate the PDF report
            var pdfBytes = _orderReportService.GenerateReport(order);

            // Return the PDF as a downloadable file
            return File(pdfBytes, "application/pdf", $"Order_{order.ID}_Report.pdf");
        }
    }
}
