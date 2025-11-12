using makatizen_app.Server.Data;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace makatizen_app.Server.Controllers
{
    // Sets the base route for the controller to /api/BiometricDataEnrollments
    [Route("api/[controller]")]
    [ApiController] // Indicates that this class is an API controller
    public class BiometricDataEnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor for Dependency Injection (injects the DbContext)
        public BiometricDataEnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // --- GET: api/BiometricDataEnrollments (Retrieve all records) ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BiometricDataEnrollment>>> GetBiometricDataEnrollments()
        {
            // The method name should match the name of the DbSet in your DbContext
            return await _context.BiometricDataEnrollments.ToListAsync();
        }

        // --- GET: api/BiometricDataEnrollments/5 (Retrieve a single record by ID) ---
        [HttpGet("{id}")]
        public async Task<ActionResult<BiometricDataEnrollment>> GetBiometricDataEnrollment(int id)
        {
            var enrollment = await _context.BiometricDataEnrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound(); // HTTP 404
            }

            return enrollment; // HTTP 200 with the enrollment data
        }

        // --- POST: api/BiometricDataEnrollments (Create a new record) ---
        [HttpPost]
        public async Task<ActionResult<BiometricDataEnrollment>> PostBiometricDataEnrollment(BiometricDataEnrollment enrollment)
        {
            _context.BiometricDataEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Returns a 201 Created status, including the new object and a URI to access it
            return CreatedAtAction(nameof(GetBiometricDataEnrollment), new { id = enrollment.Id }, enrollment);
        }

        // --- PUT: api/BiometricDataEnrollments/5 (Update an existing record) ---
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBiometricDataEnrollment(int id, BiometricDataEnrollment enrollment)
        {
            // Check if the ID in the route matches the ID in the body
            if (id != enrollment.Id)
            {
                return BadRequest(); // HTTP 400
            }

            // Tell EF Core to treat the entity as modified
            _context.Entry(enrollment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BiometricDataEnrollmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // HTTP 204 (Success, no content to return)
        }

        // --- DELETE: api/BiometricDataEnrollments/5 (Delete a record) ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBiometricDataEnrollment(int id)
        {
            var enrollment = await _context.BiometricDataEnrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.BiometricDataEnrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return NoContent(); // HTTP 204
        }

        // --- Private Helper Method ---
        private bool BiometricDataEnrollmentExists(int id)
        {
            return _context.BiometricDataEnrollments.Any(e => e.Id == id);
        }
    }
}