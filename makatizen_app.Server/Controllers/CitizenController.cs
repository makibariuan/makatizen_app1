using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Policy allows Super Admin (1), System User (2), and Kit User (3)
    [Authorize(Policy = "RequireAdminOrKit")]
    public class CitizenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitizenController(AppDbContext context)
        {
            _context = context;
        }

        // --- GET: List All Citizens with Latest Biometric Info ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitizenReadDto>>> GetCitizens()
        {
            var citizens = await _context.Citizens
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CitizenReadDto
                {
                    Id = c.Id,
                    CitizenType = c.CitizenType,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    BirthDate = c.BirthDate,
                    CreatedAt = c.CreatedAt,

                    // Fetch the latest biometric record's ID, Date, and Status
                    BiometricId = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => (int?)b.Id)
                        .FirstOrDefault(),

                    DateCapture = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => b.DateCapture)
                        .FirstOrDefault(),

                    Status = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => b.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(citizens);
        }

        // --- POST: Create New Citizen and Biometric Enrollment ---
        [HttpPost]
        public async Task<IActionResult> CreateCitizen([FromBody] CitizenCreateDto dto)
        {
            // Check if the request is valid based on DTO attributes
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Create Citizen Record
            var citizen = new Citizen
            {
                CitizenType = dto.CitizenType,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync(); // Save citizen to get the new ID

            // 2. Create Biometric Enrollment Record
            var biometric = new BiometricDataEnrollment
            {
                PersonId = citizen.Id, // Link to the newly created citizen
                DateCapture = DateTime.UtcNow, // Assuming capture occurs during creation
                DateUpload = DateTime.UtcNow,
                DateActivate = null, // Set upon external activation if needed

                // --- MAPPING BIOMETRIC FIELDS ---
                Photo = dto.Biometrics.Photo,
                Signature = dto.Biometrics.Signature,

                // LEFT HAND FINGERPRINTS
                LeftThumb = dto.Biometrics.LeftThumb,
                LeftIndex = dto.Biometrics.LeftIndex,
                LeftMiddle = dto.Biometrics.LeftMiddle, 
                LeftRing = dto.Biometrics.LeftRing,     
                LeftSmall = dto.Biometrics.LeftSmall,   

                // RIGHT HAND FINGERPRINTS
                RightThumb = dto.Biometrics.RightThumb, 
                RightIndex = dto.Biometrics.RightIndex, 
                RightMiddle = dto.Biometrics.RightMiddle, 
                RightRing = dto.Biometrics.RightRing,    
                RightSmall = dto.Biometrics.RightSmall,

                // EYE/IRIS AND GENERAL BIOMETRICS
                EyeLeft = dto.Biometrics.EyeLeft,
                EyeRight = dto.Biometrics.EyeRight,
                BiometricLeft = dto.Biometrics.BiometricLeft,
                BiometricRight = dto.Biometrics.BiometricRight,

                Hit = 0,
                Status = dto.Biometrics.Status ?? 1 // Default status to 1 (Active)
            };

            _context.BiometricEnrollments.Add(biometric);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCitizens), new { id = citizen.Id }, new
            {
                message = "Citizen and Biometric data successfully enrolled.",
                citizenId = citizen.Id,
                biometricId = biometric.Id
            });
        }

        // --- GET: Get specific citizen with detailed biometric history ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCitizen(int id)
        {
            var citizen = await _context.Citizens
                .Include(c => c.BiometricEnrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found." });
            }

            // To be thorough, you would define a detailed CitizenDetailDto 
            // that includes the full list of BiometricEnrollment records.
            // For now, we return the entity directly (in a real app, always use DTOs).
            return Ok(citizen);
        }

        // --- PUT: Update Citizen Personal Details (Optional: Update/Add Biometric Data) ---
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCitizen(int id, [FromBody] CitizenCreateDto dto)
        {
            var citizen = await _context.Citizens
                .Include(c => c.BiometricEnrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found." });
            }

            // Update personal details
            citizen.CitizenType = dto.CitizenType;
            citizen.FirstName = dto.FirstName;
            citizen.LastName = dto.LastName;
            citizen.BirthDate = dto.BirthDate;

            // Optional: If the PUT request includes new biometric data, create a new enrollment record
            if (dto.Biometrics != null)
            {
                var newBiometric = new BiometricDataEnrollment
                {
                    PersonId = citizen.Id,
                    DateCapture = DateTime.UtcNow,
                    DateUpload = DateTime.UtcNow,
                    // Map all fields from DTO
                    Photo = dto.Biometrics.Photo,
                    Signature = dto.Biometrics.Signature,
                    // ... other fields ...
                    Status = dto.Biometrics.Status ?? 1
                };
                _context.BiometricEnrollments.Add(newBiometric);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // --- DELETE: Delete Citizen and associated Biometrics ---
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitizen(int id)
        {
            var citizen = await _context.Citizens
                .Include(c => c.BiometricEnrollments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (citizen == null)
            {
                return NotFound(new { message = "Citizen not found." });
            }
            _context.Citizens.Remove(citizen);

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}