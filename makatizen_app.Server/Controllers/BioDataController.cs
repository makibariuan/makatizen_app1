//using makatizen_app.Server.Data;
//using makatizen_app.Server.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace makatizen_app.Server.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]

//    //[AllowAnonymous]
//    public class BiometricDataEnrollmentsController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public BiometricDataEnrollmentsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // --- GET:(Retrieve all records) ---
//        // NOTE: Returning the raw entity here may cause a JSON cycle if the BiometricDataEnrollment model
//        // includes a navigation property back to Citizen. It is recommended to use BiometricReadDto instead.
//        [HttpGet("citizen-enrollments")]
//        public async Task<ActionResult<IEnumerable<BiometricDataEnrollment>>> GetBiometricDataEnrollments()
//        {
//            return await _context.BiometricEnrollments.ToListAsync();
//        }

//        // PersonId = CitizenId
//        [HttpGet("citizen-enrollments/{personId}")]
//        public async Task<ActionResult<IEnumerable<BiometricDataEnrollment>>> GetEnrollmentsByCitizenId(int personId)
//        {
//            var enrollments = await _context.BiometricEnrollments
//                .Where(e => e.PersonId == personId)
//                .OrderByDescending(e => e.DateCapture)
//                .ToListAsync();

//            if (!enrollments.Any())
//            {
//                return NotFound($"No biometric enrollments found for Citizen ID {personId}.");
//            }

//            return enrollments;
//        }

//        // --- GET:(Retrieve a single record by ID) ---
//        [HttpGet("citizen-enrollment/{id}")]
//        public async Task<ActionResult<BiometricDataEnrollment>> GetBiometricDataEnrollment(int id)
//        {
//            var enrollment = await _context.BiometricEnrollments.FindAsync(id);

//            if (enrollment == null)
//            {
//                return NotFound();
//            }

//            return enrollment;
//        }

//        // --- POST:(Create a new record) ---
//        [HttpPost("create-new-enrollment")]
//        public async Task<ActionResult<BiometricDataEnrollment>> PostBiometricDataEnrollment(BiometricDataEnrollment enrollment)
//        {
//            if (enrollment.PersonId <= 0)
//            {
//                return BadRequest("The PersonId field is required and must be valid.");
//            }

//            // NOTE: In a production app, you would likely use a DTO here (e.g., BiometricCreateDto) 
//            // to prevent over-posting and then map it to the model.
//            _context.BiometricEnrollments.Add(enrollment);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetBiometricDataEnrollment), new { id = enrollment.Id }, enrollment);
//        }

//        // --- PUT:(Update an existing record) ---
//        [HttpPut("update-enrollment/{id}")]
//        public async Task<IActionResult> PutBiometricDataEnrollment(int id, BiometricDataEnrollment enrollment)
//        {
//            if (id != enrollment.Id)
//            {
//                return BadRequest();
//            }
//            // NOTE: For safety and performance, a better approach is to fetch the existing entity, 
//            // update only the allowed properties, and save changes, rather than setting the EntityState to Modified.
//            _context.Entry(enrollment).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BiometricDataEnrollmentExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // --- DELETE:(Delete a record) ---
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteBiometricDataEnrollment(int id)
//        {
//            var enrollment = await _context.BiometricEnrollments.FindAsync(id);
//            if (enrollment == null)
//            {
//                return NotFound();
//            }

//            _context.BiometricEnrollments.Remove(enrollment);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        // Private helper method
//        private bool BiometricDataEnrollmentExists(int id)
//        {
//            return _context.BiometricEnrollments.Any(e => e.Id == id);
//        }
//    }
//}