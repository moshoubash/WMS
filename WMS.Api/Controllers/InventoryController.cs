using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<IActionResult> GetInventories()
        {
            var inventories = await _context.Inventories.ToListAsync();
            return Ok(inventories);
        }

        // GET: api/inventory/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return Ok(inventory);
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventory), new { id = inventory.ID }, inventory);
        }

        // PUT: api/inventory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] Inventory inventory)
        {
            if (id != inventory.ID)
            {
                return BadRequest();
            }

            // Load the existing inventory with related data (Address, ContactInfo)
            var existingInventory = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (existingInventory == null)
            {
                return NotFound();
            }

            // Update the inventory data
            existingInventory.Quantity = inventory.Quantity;

            // Update OrderID and ShipmentID directly
            existingInventory.ProductId = inventory.ProductId;
            existingInventory.WarehouseID = inventory.WarehouseID;

            // Mark the main entity and related entities as modified
            _context.Entry(existingInventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Inventories.Any(e => e.ID == id))
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


        // DELETE: api/inventory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchInventory(string Query)
        {
            var inventories = await _context.Inventories.Where(w => w.ID.ToString().Contains(Query)).FirstOrDefaultAsync();

            if (inventories == null)
            {
                return NotFound();
            }

            return Ok(inventories);
        }
    }
}
