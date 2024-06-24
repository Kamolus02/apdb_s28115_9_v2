using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controller;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly S28115Context _context;

    public TripsController(S28115Context context)
    {
        _context = context;
    }

    // GET: api/trips
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TripDto>>> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var tripsQuery = _context.Trips
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto { Name = c.Name }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            });

        var trips = await tripsQuery.ToListAsync();

        var totalTrips = await _context.Trips.CountAsync();

        return Ok(new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = (int)Math.Ceiling(totalTrips / (double)pageSize),
            trips
        });
    }
}