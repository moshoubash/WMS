using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;
namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        protected readonly ApplicationDbContext _dbContext;

        public AddressController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/address
        [HttpGet]
        [AllowAnonymous]
        [Authorize]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await _dbContext.Addresses.ToListAsync();
            return Ok(addresses);
        }

        // GET: api/address/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var address = await _dbContext.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        // POST: api/address
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] Address address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _dbContext.Addresses.Add(address);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.ID }, address);
        }

        // PUT: api/address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] Address address)
        {
            if (id != address.ID)
            {
                return BadRequest("You try to update address that is not accessible or not exist!");
            }

            _dbContext.Entry(address).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Addresses.Any(e => e.ID == id))
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

        // DELETE: api/address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _dbContext.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/address/{query}
        [HttpGet("query")]
        public async Task<IActionResult> SearchAddress(string Query)
        {
            var addresses = await _dbContext.Addresses.Where(a => a.Country.Contains(Query) 
                                                                || a.City.Contains(Query)
                                                                || a.ID.ToString().Contains(Query)
                                                                || a.State.Contains(Query)
                                                                || a.Street.Contains(Query)
                                                                || a.PostalCode.ToString().Contains(Query)).ToListAsync();

            if (addresses == null)
            {
                return NotFound();
            }

            return Ok(addresses);
        }
    }
}
