using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingApp.API.Data;
using MarketingApp.API.DTOs;
using MarketingApp.API.Models;

namespace MarketingApp.API.Controllers
{
    /// <summary>
    /// REST API controller for Personnel CRUD operations.
    /// Base route: /api/personnel
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PersonnelController : ControllerBase
    {
        private readonly MarketingDbContext _context;

        // Constructor injection – ASP.NET Core DI provides the DbContext
        public PersonnelController(MarketingDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------
        // GET /api/personnel
        // Returns all personnel records (no sales data included here).
        // ----------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonnelDto>>> GetAll()
        {
            var list = await _context.Personnel
                .OrderBy(p => p.Name)
                .Select(p => new PersonnelDto
                {
                    Id    = p.Id,
                    Name  = p.Name,
                    Age   = p.Age,
                    Phone = p.Phone
                })
                .ToListAsync();

            return Ok(list);
        }

        // ----------------------------------------------------------------
        // GET /api/personnel/{id}
        // Returns a single personnel record by Id.
        // ----------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonnelDto>> GetById(int id)
        {
            var person = await _context.Personnel.FindAsync(id);

            if (person == null)
                return NotFound(new { message = $"Personnel with Id {id} not found." });

            return Ok(new PersonnelDto
            {
                Id    = person.Id,
                Name  = person.Name,
                Age   = person.Age,
                Phone = person.Phone
            });
        }

        // ----------------------------------------------------------------
        // POST /api/personnel
        // Creates a new personnel record.
        // Validation is handled by DataAnnotations on PersonnelUpsertDto.
        // ----------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<PersonnelDto>> Create([FromBody] PersonnelUpsertDto dto)
        {
            // ModelState is automatically validated by [ApiController]
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = new Personnel
            {
                Name  = dto.Name.Trim(),
                Age   = dto.Age,
                Phone = dto.Phone.Trim()
            };

            _context.Personnel.Add(person);
            await _context.SaveChangesAsync();

            // Return 201 Created with the location header pointing to the new resource
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, new PersonnelDto
            {
                Id    = person.Id,
                Name  = person.Name,
                Age   = person.Age,
                Phone = person.Phone
            });
        }

        // ----------------------------------------------------------------
        // PUT /api/personnel/{id}
        // Updates an existing personnel record.
        // ----------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult<PersonnelDto>> Update(int id, [FromBody] PersonnelUpsertDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var person = await _context.Personnel.FindAsync(id);

            if (person == null)
                return NotFound(new { message = $"Personnel with Id {id} not found." });

            // Apply changes
            person.Name  = dto.Name.Trim();
            person.Age   = dto.Age;
            person.Phone = dto.Phone.Trim();

            await _context.SaveChangesAsync();

            return Ok(new PersonnelDto
            {
                Id    = person.Id,
                Name  = person.Name,
                Age   = person.Age,
                Phone = person.Phone
            });
        }

        // ----------------------------------------------------------------
        // DELETE /api/personnel/{id}
        // Deletes a personnel record and all related sales (via CASCADE).
        // The confirmation dialog is handled on the frontend – the API
        // simply performs the deletion when called.
        // ----------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.Personnel.FindAsync(id);

            if (person == null)
                return NotFound(new { message = $"Personnel with Id {id} not found." });

            _context.Personnel.Remove(person);
            await _context.SaveChangesAsync();

            // 204 No Content is the standard response for a successful DELETE
            return NoContent();
        }
    }
}
