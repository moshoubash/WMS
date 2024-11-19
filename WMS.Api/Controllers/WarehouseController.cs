using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/warehouse
        [HttpGet]
        public async Task<IActionResult> GetWarehouses()
        {
            var warehouses = await _context.Warehouses.ToListAsync();
            return Ok(warehouses);
        }

        // GET: api/warehouse/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        // POST: api/warehouse
        [HttpPost]
        public async Task<IActionResult> CreateWarehouse([FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarehouse", new { id = warehouse.ID }, warehouse);
        }

        // PUT: api/warehouse/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            if (id != warehouse.ID)
            {
                return BadRequest();
            }

            // Load the existing warehouse with related data (Address, ContactInfo)
            var existingWarehouse = await _context.Warehouses
                .Include(w => w.Address)
                .Include(w => w.ContactInfo)
                .FirstOrDefaultAsync(w => w.ID == id);

            if (existingWarehouse == null)
            {
                return NotFound();
            }

            // Update the warehouse data
            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Capacity = warehouse.Capacity;

            // Update the Address data
            var address = _context.Addresses.Where(a => a.ID == existingWarehouse.AddressId).FirstOrDefault();
            var contact = _context.ContactInfos.Where(ci => ci.ID == existingWarehouse.ContactInfoId).FirstOrDefault();

            if (address != null && warehouse.Address != null)
            {
                address.Country = warehouse.Address.Country;
                address.State = warehouse.Address.State;
                address.City = warehouse.Address.City;
                address.Street = warehouse.Address.Street;
                address.PostalCode = warehouse.Address.PostalCode;
                address.Info = warehouse.Address.Info;
            }

            // Update the ContactInfo data
            if (contact != null && warehouse.ContactInfo != null)
            {
                contact.Email = warehouse.ContactInfo.Email;
                contact.PhoneNumber = warehouse.ContactInfo.PhoneNumber;
            }

            // Mark the main entity and related entities as modified
            _context.Entry(existingWarehouse).State = EntityState.Modified;
            _context.Entry(address).State = EntityState.Modified;
            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Warehouses.Any(e => e.ID == id))
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


        // DELETE: api/warehouse/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchWarehouse(string Query)
        {
            var warehouses = await _context.Warehouses.Where(w => w.ID.ToString().Contains(Query) || w.Name.Contains(Query)).FirstOrDefaultAsync();

            if (warehouses == null)
            {
                return NotFound();
            }

            return Ok(warehouses);
        }
    }
}
