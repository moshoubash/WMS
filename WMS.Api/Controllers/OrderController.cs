using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        protected readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            // var orders = await _dbContext.Orders.Include(o => o.customer).ToListAsync();
            var orders = await _dbContext.Orders.ToListAsync();
            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }


        // POST: api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.ID }, order);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.ID)
            {
                return BadRequest();
            }

            // Load the existing order with related data (Address, ContactInfo)
            var existingOrder = await _dbContext.Orders
                .Include(w => w.Address)
                .Include(w => w.Customer)
                .FirstOrDefaultAsync(w => w.ID == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Update the order data
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.Status = order.Status;
            existingOrder.TotalAmount = order.TotalAmount;

            // Update the Address data
            var address = _dbContext.Addresses.Where(a => a.ID == existingOrder.AddressId).FirstOrDefault();
            var customer = _dbContext.Customers.Where(c => c.ID == existingOrder.CustomerID).FirstOrDefault();

            if (address != null && order.Address != null)
            {
                address.Country = order.Address.Country;
                address.State = order.Address.State;
                address.City = order.Address.City;
                address.Street = order.Address.Street;
                address.PostalCode = order.Address.PostalCode;
                address.Info = order.Address.Info;
            }

            // Update the ContactInfo data
            if (customer != null && order.Customer != null)
            {
                customer.Name = order.Customer.Name;
            }

            // Mark the main entity and related entities as modified
            _dbContext.Entry(existingOrder).State = EntityState.Modified;
            _dbContext.Entry(address).State = EntityState.Modified;
            _dbContext.Entry(customer).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Orders.Any(e => e.ID == id))
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

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchOrder(string Query)
        {
            var order = await _dbContext.Orders.Where(o => o.ID.ToString().Contains(Query)).FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
