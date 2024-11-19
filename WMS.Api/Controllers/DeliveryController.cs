using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        protected readonly ApplicationDbContext _dbContext;

        public DeliveryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/delivery
        [HttpGet]
        public async Task<IActionResult> GetDeliveries()
        {
            var deliveries = await _dbContext.Deliveries.ToListAsync();
            return Ok(deliveries);
        }

        // GET: api/delivery/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDelivery(int id)
        {
            var delivery = await _dbContext.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }

        // POST: api/delivery
        [HttpPost]
        public async Task<IActionResult> CreateDelivery([FromBody] Delivery delivery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Deliveries.Add(delivery);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetDeliveries", new { id = delivery.ID }, delivery);
        }

        // PUT: api/delivery/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(int id, [FromBody] Delivery delivery)
        {
            if (id != delivery.ID)
            {
                return BadRequest();
            }

            // Load the existing delivery with related data (Address, ContactInfo)
            var existingDelivery = await _dbContext.Deliveries
                .Include(d => d.Order)
                .Include(d => d.Shipment)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (existingDelivery == null)
            {
                return NotFound();
            }

            // Update the delivery data
            existingDelivery.Date = delivery.Date;
            existingDelivery.Status = delivery.Status;
            existingDelivery.DeliveryPerson = delivery.DeliveryPerson;

            // Update OrderID and ShipmentID directly
            existingDelivery.OrderID = delivery.OrderID;
            existingDelivery.ShipmentID = delivery.ShipmentID;

            // Mark the main entity and related entities as modified
            _dbContext.Entry(existingDelivery).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Warehouses.Any(e => e.ID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/delivery/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery = await _dbContext.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            _dbContext.Deliveries.Remove(delivery);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/delivery/{query}
        [HttpGet("query")]
        public async Task<IActionResult> SearchDelivery(string Query)
        {
            var deliveries = await _dbContext.Deliveries.Where(d => d.Status.Contains(Query) || d.DeliveryPerson.Contains(Query)).ToListAsync();

            if (deliveries == null)
            {
                return NotFound();
            }

            return Ok(deliveries);
        }
    }
}
