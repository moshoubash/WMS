using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        protected readonly ApplicationDbContext _dbContext;

        public SupplierController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/supplier
        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _dbContext.Suppliers.ToListAsync();
            return Ok(suppliers);
        }

        // GET: api/supplier/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await _dbContext.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // POST: api/supplier
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Suppliers.Add(supplier);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetSuppliers", new { id = supplier.ID }, supplier);
        }

        // PUT: api/supplier/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.ID)
            {
                return BadRequest();
            }

            // Load the existing supplier with related data (Address, ContactInfo)
            var existingSupplier = await _dbContext.Suppliers
                .Include(w => w.ContactInfo)
                .FirstOrDefaultAsync(w => w.ID == id);

            if (existingSupplier == null)
            {
                return NotFound();
            }

            var contact = _dbContext.ContactInfos.Where(ci => ci.ID == existingSupplier.ContactInfoId).FirstOrDefault();

            // Update the supplier data
            existingSupplier.Name = supplier.Name;

            // Update the ContactInfo data
            if (contact != null && supplier.ContactInfo != null)
            {
                contact.Email = supplier.ContactInfo.Email;
                contact.PhoneNumber = supplier.ContactInfo.PhoneNumber;
            }

            // Mark the main entity and related entities as modified
            _dbContext.Entry(existingSupplier).State = EntityState.Modified;
            _dbContext.Entry(contact).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Suppliers.Any(e => e.ID == id))
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

        // DELETE: api/supplier/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _dbContext.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            _dbContext.Suppliers.Remove(supplier);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/supplier/{query}
        [HttpGet("query")]
        public async Task<IActionResult> SearchSupplier(string Query)
        {
            var suppliers = await _dbContext.Suppliers.Where(c => c.ID.ToString().Contains(Query) || c.Name.Contains(Query)).ToListAsync();

            if (suppliers == null)
            {
                return NotFound();
            }

            return Ok(suppliers);
        }
    }
}
