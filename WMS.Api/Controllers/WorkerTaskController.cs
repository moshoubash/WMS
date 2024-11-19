using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Core;

namespace WMS.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WorkerTaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkerTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/workerTask
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var workerTasks = await _context.Tasks.ToListAsync();
            return Ok(workerTasks);
        }

        // GET: api/workerTask/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkerTask(int id)
        {
            var workerTask = await _context.Tasks.FindAsync(id);

            if (workerTask == null)
            {
                return NotFound();
            }

            return Ok(workerTask);
        }

        // POST: api/workerTask
        [HttpPost]
        public async Task<IActionResult> CreateWorkerTask([FromBody] WorkerTask workerTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tasks.Add(workerTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkerTask", new { id = workerTask.Id }, workerTask);
        }

        // PUT: api/workerTask/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkerTask(int id, [FromBody] WorkerTask workerTask)
        {
            if (id != workerTask.Id)
            {
                return BadRequest();
            }

            // Load the existing workerTask with related data (Address, ContactInfo)
            var existingWorkerTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (existingWorkerTask == null)
            {
                return NotFound();
            }

            // Update the workerTask data
            existingWorkerTask.Name = workerTask.Name;
            existingWorkerTask.Description = workerTask.Description;
            existingWorkerTask.UserId = workerTask.UserId;

            // Mark the main entity and related entities as modified
            _context.Entry(existingWorkerTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tasks.Any(t => t.Id == id))
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


        // DELETE: api/workerTask/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkerTask(int id)
        {
            var workerTask = await _context.Tasks.FindAsync(id);
            if (workerTask == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(workerTask);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("query")]
        public async Task<IActionResult> SearchWorkerTask(string Query)
        {
            var workerTasks = await _context.Tasks.Where(t => t.Id.ToString().Contains(Query) || t.Name.Contains(Query) || t.UserId.Contains(Query)).FirstOrDefaultAsync();

            if (workerTasks == null)
            {
                return NotFound();
            }

            return Ok(workerTasks);
        }
    }
}
