using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/kitusers/[controller]")]

    [Authorize]
    //[AllowAnonymous] 
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

                    // Select latest biometric metadata using EF Core in-memory sorting
                    BiometricId = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => (int?)b.Id)
                        .FirstOrDefault(),

                    DateCapture = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => (DateTime?)b.DateCapture)
                        .FirstOrDefault(),

                    Status = c.BiometricEnrollments
                        .OrderByDescending(b => b.DateUpload)
                        .Select(b => (int?)b.Status)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(citizens);
        }

        // --- GET: Get specific citizen with detailed biometric history (Refactored to use DTOs) ---
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
            var detailDto = new CitizenDetailDto
            {
                Id = citizen.Id,
                CitizenType = citizen.CitizenType,
                FirstName = citizen.FirstName,
                LastName = citizen.LastName,
                BirthDate = citizen.BirthDate,
                CreatedAt = citizen.CreatedAt,
                BiometricHistory = citizen.BiometricEnrollments
                    .Select(b => new BiometricReadDto
                    {
                        Id = b.Id,
                        DateCapture = b.DateCapture ?? DateTime.MinValue,
                        DateUpload = b.DateUpload ?? DateTime.MinValue,
                        DateActivate = b.DateActivate,
                        Status = b.Status, 

                        // Map all biometric fields
                        Photo = b.Photo,
                        Signature = b.Signature,
                        LeftThumb = b.LeftThumb,
                        LeftIndex = b.LeftIndex,
                        LeftMiddle = b.LeftMiddle,
                        LeftRing = b.LeftRing,
                        LeftSmall = b.LeftSmall,
                        RightThumb = b.RightThumb,
                        RightIndex = b.RightIndex,
                        RightMiddle = b.RightMiddle,
                        RightRing = b.RightRing,
                        RightSmall = b.RightSmall,
                        EyeLeft = b.EyeLeft,
                        EyeRight = b.EyeRight,
                        BiometricLeft = b.BiometricLeft,
                        BiometricRight = b.BiometricRight
                    })
                    .OrderByDescending(b => b.DateCapture)
                    .ToList()
            };

            // 4. Return the DTO
            return Ok(detailDto);
        }

        // --- POST: Create New Citizen and Biometric Enrollment ---
        [HttpPost]
        public async Task<IActionResult> CreateCitizen([FromBody] CitizenCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            // Biometrics is required for creation via this endpoint
            if (dto.Biometrics == null)
            {
                return BadRequest(new { message = "Biometric data is required for initial enrollment." });
            }

            var biometric = new BiometricDataEnrollment
            {
                PersonId = citizen.Id,
                DateCapture = DateTime.UtcNow,
                DateUpload = DateTime.UtcNow,
                DateActivate = null,

                // --- MAPPING BIOMETRIC FIELDS ---
                Photo = dto.Biometrics.Photo,
                Signature = dto.Biometrics.Signature,
                LeftThumb = dto.Biometrics.LeftThumb,
                LeftIndex = dto.Biometrics.LeftIndex,
                LeftMiddle = dto.Biometrics.LeftMiddle,
                LeftRing = dto.Biometrics.LeftRing,
                LeftSmall = dto.Biometrics.LeftSmall,
                RightThumb = dto.Biometrics.RightThumb,
                RightIndex = dto.Biometrics.RightIndex,
                RightMiddle = dto.Biometrics.RightMiddle,
                RightRing = dto.Biometrics.RightRing,
                RightSmall = dto.Biometrics.RightSmall,
                EyeLeft = dto.Biometrics.EyeLeft,
                EyeRight = dto.Biometrics.EyeRight,
                BiometricLeft = dto.Biometrics.BiometricLeft,
                BiometricRight = dto.Biometrics.BiometricRight,

                Hit = 0,
                Status = dto.Biometrics.Status ?? 1 // Default status to 1 (Active)
            };

            _context.BiometricEnrollments.Add(biometric);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCitizen), new { id = citizen.Id }, new
            {
                message = "Citizen and Biometric data successfully enrolled.",
                citizenId = citizen.Id,
                biometricId = biometric.Id
            });
        }

        // --- PUT: Update Citizen Personal Details and optionally add a new Biometric Enrollment ---
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

            // --- Update Personal Details ---
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
                    DateActivate = null,

                    // --- MAPPING BIOMETRIC FIELDS ---
                    Photo = dto.Biometrics.Photo,
                    Signature = dto.Biometrics.Signature,
                    LeftThumb = dto.Biometrics.LeftThumb,
                    LeftIndex = dto.Biometrics.LeftIndex,
                    LeftMiddle = dto.Biometrics.LeftMiddle,
                    LeftRing = dto.Biometrics.LeftRing,
                    LeftSmall = dto.Biometrics.LeftSmall,
                    RightThumb = dto.Biometrics.RightThumb,
                    RightIndex = dto.Biometrics.RightIndex,
                    RightMiddle = dto.Biometrics.RightMiddle,
                    RightRing = dto.Biometrics.RightRing,
                    RightSmall = dto.Biometrics.RightSmall,
                    EyeLeft = dto.Biometrics.EyeLeft,
                    EyeRight = dto.Biometrics.EyeRight,
                    BiometricLeft = dto.Biometrics.BiometricLeft,
                    BiometricRight = dto.Biometrics.BiometricRight,

                    Hit = 0, // Default value
                    Status = dto.Biometrics.Status ?? 1 // Default status to 1 if null
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
                // BiometricEnrollments should ideally be cascade deleted in the database configuration
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