using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        protected readonly ApplicationDbContext _dbContext;

        public CustomerController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _dbContext.Customers.ToListAsync();
            return Ok(customers);
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // POST: api/customer
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetCustomers", new { id = customer.ID }, customer);
        }

        // PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            if (id != customer.ID)
            {
                return BadRequest();
            }

            // Load the existing customer with related data (Address, ContactInfo)
            var existingCustomer = await _dbContext.Customers
                .Include(w => w.Address)
                .Include(w => w.ContactInfo)
                .FirstOrDefaultAsync(w => w.ID == id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Update the customer data
            existingCustomer.Name = customer.Name;

            // Update the Address data
            var address = _dbContext.Addresses.Where(a => a.ID == existingCustomer.AddressId).FirstOrDefault();
            var contact = _dbContext.ContactInfos.Where(ci => ci.ID == existingCustomer.ContactInfoId).FirstOrDefault();

            if (address != null && customer.Address != null)
            {
                address.Country = customer.Address.Country;
                address.State = customer.Address.State;
                address.City = customer.Address.City;
                address.Street = customer.Address.Street;
                address.PostalCode = customer.Address.PostalCode;
                address.Info = customer.Address.Info;
            }

            // Update the ContactInfo data
            if (contact != null && customer.ContactInfo != null)
            {
                contact.Email = customer.ContactInfo.Email;
                contact.PhoneNumber = customer.ContactInfo.PhoneNumber;
            }

            // Mark the main entity and related entities as modified
            _dbContext.Entry(existingCustomer).State = EntityState.Modified;
            _dbContext.Entry(address).State = EntityState.Modified;
            _dbContext.Entry(contact).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Customers.Any(e => e.ID == id))
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

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/customer/{query}
        [HttpGet("query")]
        public async Task<IActionResult> SearchCustomer(string Query)
        {
            var customers = await _dbContext.Customers.Where(c => c.ID.ToString().Contains(Query) || c.Name.Contains(Query)).ToListAsync();

            if (customers == null)
            {
                return NotFound();
            }

            return Ok(customers);
        }
    }
}
