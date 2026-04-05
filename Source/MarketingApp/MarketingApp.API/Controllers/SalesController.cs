using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingApp.API.Data;
using MarketingApp.API.DTOs;
using MarketingApp.API.Models;

namespace MarketingApp.API.Controllers
{
    /// <summary>
    /// REST API controller for Sales records.
    /// Sales can only be ADDED or DELETED – editing is intentionally not supported.
    /// Base route: /api/personnel/{personnelId}/sales
    /// Using a nested route keeps the URL structure RESTful.
    /// </summary>
    [ApiController]
    [Route("api/personnel/{personnelId:int}/sales")]
    public class SalesController : ControllerBase
    {
        private readonly MarketingDbContext _context;

        public SalesController(MarketingDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------------------
        // GET /api/personnel/{personnelId}/sales
        // Returns all sales records for a given personnel member,
        // ordered by date ascending (good for charting).
        // ----------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDto>>> GetByPersonnel(int personnelId)
        {
            // Confirm the personnel exists first
            bool exists = await _context.Personnel.AnyAsync(p => p.Id == personnelId);
            if (!exists)
                return NotFound(new { message = $"Personnel with Id {personnelId} not found." });

            var sales = await _context.Sales
                .Where(s => s.PersonnelId == personnelId)
                .OrderBy(s => s.ReportDate)
                .Select(s => new SaleDto
                {
                    Id          = s.Id,
                    PersonnelId = s.PersonnelId,
                    ReportDate  = s.ReportDate,
                    SalesAmount = s.SalesAmount
                })
                .ToListAsync();

            return Ok(sales);
        }

        // ----------------------------------------------------------------
        // POST /api/personnel/{personnelId}/sales
        // Adds a new sales record for the specified personnel.
        // ----------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<SaleDto>> Create(int personnelId, [FromBody] SaleCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Confirm the personnel exists
            bool exists = await _context.Personnel.AnyAsync(p => p.Id == personnelId);
            if (!exists)
                return NotFound(new { message = $"Personnel with Id {personnelId} not found." });

            var sale = new Sale
            {
                PersonnelId = personnelId,
                ReportDate  = dto.ReportDate,
                SalesAmount = dto.SalesAmount
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByPersonnel), new { personnelId }, new SaleDto
            {
                Id          = sale.Id,
                PersonnelId = sale.PersonnelId,
                ReportDate  = sale.ReportDate,
                SalesAmount = sale.SalesAmount
            });
        }

        // ----------------------------------------------------------------
        // DELETE /api/personnel/{personnelId}/sales/{saleId}
        // Deletes a single sales record.
        // ----------------------------------------------------------------
        [HttpDelete("{saleId:int}")]
        public async Task<IActionResult> Delete(int personnelId, int saleId)
        {
            var sale = await _context.Sales
                .FirstOrDefaultAsync(s => s.Id == saleId && s.PersonnelId == personnelId);

            if (sale == null)
                return NotFound(new { message = $"Sale record {saleId} not found for personnel {personnelId}." });

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
