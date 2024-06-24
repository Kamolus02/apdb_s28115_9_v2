using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controller;

[Route("api/trips/{idTrip}/clients")]
[ApiController]
public class TripClientsController : ControllerBase
{
    private readonly S28115Context _context;

    public TripClientsController(S28115Context context)
    {
        _context = context;
    }

    // POST: api/trips/5/clients
    [HttpPost]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] AddClientDto addClientDto)
    {
        var trip = await _context.Trips.FindAsync(idTrip);

        if (trip == null || trip.DateFrom <= DateTime.Now)
        {
            return BadRequest("Trip does not exist or has already started.");
        }

        var existingClient = await _context.Clients
            .FirstOrDefaultAsync(c => c.Pesel == addClientDto.Pesel);

        if (existingClient != null)
        {
            var isClientRegistered = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (isClientRegistered)
            {
                return BadRequest("Client is already registered for this trip.");
            }
        }
        else
        {
            existingClient = new Client
            {
                FirstName = addClientDto.FirstName,
                LastName = addClientDto.LastName,
                Email = addClientDto.Email,
                Telephone = addClientDto.Telephone,
                Pesel = addClientDto.Pesel
            };
            _context.Clients.Add(existingClient);
            await _context.SaveChangesAsync();
        }

        var clientTrip = new ClientTrip
        {
            IdClient = existingClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = addClientDto.PaymentDate
        };

        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
