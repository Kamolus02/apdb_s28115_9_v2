using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;

namespace WebApplication1.Controller;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly S28115Context _context;

    public ClientsController(S28115Context context)
    {
        _context = context;
    }

    // DELETE: api/clients/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _context.Clients.FindAsync(id);

        if (client == null)
        {
            return NotFound();
        }

        var hasTrips = await _context.ClientTrips.AnyAsync(ct => ct.IdClient == id);

        if (hasTrips)
        {
            return BadRequest("Client cannot be deleted because they are registered for one or more trips.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}