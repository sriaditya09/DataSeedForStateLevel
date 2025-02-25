using DataSeedForStateLevel.Data;
using DataSeedForStateLevel.DTO;
using DataSeedForStateLevel.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataSeedForStateLevel.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DistrictController : ControllerBase
	{
		private readonly DataDbContext _context;

		public DistrictController(DataDbContext context)
		{
			_context = context;
		}

		// GET api/district/{districtCode}
		[HttpGet("{districtCode}")]
		public async Task<ActionResult<IEnumerable<DistrictResponseDto>>> GetDistrictByDistrictCode(int districtCode)
		{
			// Query the District table by the DistrictCode
			var district = await _context.District
				.Where(d => d.DistrictCode == districtCode)
				.FirstOrDefaultAsync();

			if (district == null)
			{
				return NotFound(new { message = "District not found" });
			}

			// Create the response DTO from the found district
			var districtResponse = new DistrictResponseDto
			{
				State = district.state, // If 'state' is a string in the District model
				District = district.district,
				DistrictCode = district.DistrictCode,
				StateCode = district.StateCode
			};

			return Ok(new List<DistrictResponseDto> { districtResponse });
		}

		[HttpGet("state/{stateCode}")]
		public async Task<ActionResult<IEnumerable<DistrictResponseDto>>> GetDistrictsByStateCode(int stateCode)
		{
			// Query the District table by StateCode
			var districts = await _context.District
				.Where(d => d.StateCode == stateCode)
				.ToListAsync();

			// If no districts are found, return NotFound
			if (districts == null || !districts.Any())
			{
				return NotFound(new { message = "No districts found for the given StateCode" });
			}

			// Convert the result into a DTO (DistrictResponseDto) for the response
			var districtResponse = districts.Select(d => new DistrictResponseDto
			{
				State = d.state,
				District = d.district,
				DistrictCode = d.DistrictCode,
				StateCode = d.StateCode
			}).ToList();

			return Ok(districtResponse);
		}
	}
}

