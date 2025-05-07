using APBDCW7.Exceptions;
using APBDCW7.Models.DTOs;
using APBDCW7.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBDCW7.Controllers;


[ApiController]
[Route("api/[controller]")] // api/clients
public class ClientsController(IDbService dbService) : ControllerBase
{

    // zwraca wszystkie wycieczki danego klienta
    [HttpGet]
    [Route("{clientId}/trips")]
    public async Task<IActionResult> GetAllClientTripsAsync([FromRoute] int clientId)
    {
        try
        {
            return Ok(await dbService.GetClientTripsAsync(clientId));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    

    // tworzy nowego klienta
    [HttpPost]
    public async Task<IActionResult> CreateClientAsync([FromBody] ClientCreateDTO clientBody)
    {
        var result = await dbService.CreateClientAsync(clientBody);
        return Created($"result.IdClient", result);
    }
    

    // rejestruje klienta na wycieczke
    [HttpPut]
    [Route("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientToTripAsync([FromRoute] int clientId, [FromRoute] int tripId)
    {
        try
        {
            await dbService.RegisterClientToTripAsync(clientId, tripId);
            return NoContent();
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        
    }

    
    // usuwa rejestracje klienta z wycieczki
    [HttpDelete]
    [Route("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTripAsync([FromRoute] int clientId, int tripId)
    {
        try
        {
            await dbService.RemoveClientFromTripAsync(clientId, tripId);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
}
