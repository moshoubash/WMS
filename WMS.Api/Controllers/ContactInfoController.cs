using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/contactinfo
        [HttpGet]
        public async Task<IActionResult> GetContactInfos()
        {
            var contactInfos = await _context.ContactInfos.ToListAsync();
            return Ok(contactInfos);
        }

        // GET: api/contactinfo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactInfo(int id)
        {
            var contactInfo = await _context.ContactInfos.FindAsync(id);

            if (contactInfo == null)
            {
                return NotFound();
            }

            return Ok(contactInfo);
        }

        // POST: api/contactinfo
        [HttpPost]
        public async Task<IActionResult> CreateContactInfo([FromBody] ContactInfo contactInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactInfos.Add(contactInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactInfo", new { id = contactInfo.ID }, contactInfo);
        }

        // PUT: api/contactinfo/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactInfo(int id, [FromBody] ContactInfo contactInfo)
        {
            if (id != contactInfo.ID)
            {
                return BadRequest();
            }

            _context.Entry(contactInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ContactInfos.Any(e => e.ID == id))
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

        // DELETE: api/contactinfo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactInfo(int id)
        {
            var contactInfo = await _context.ContactInfos.FindAsync(id);
            if (contactInfo == null)
            {
                return NotFound();
            }

            _context.ContactInfos.Remove(contactInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/contactinfo/{query}
        [HttpGet("query")]
        public async Task<IActionResult> SearchContact(string Query)
        {
            var contacts = await _context.ContactInfos.Where(ci => ci.ID.ToString().Contains(Query) || ci.Email.Contains(Query) || ci.PhoneNumber.Contains(Query)).ToListAsync();

            if (contacts == null)
            {
                return NotFound();
            }

            return Ok(contacts);
        }
    }
}
