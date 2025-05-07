using APBDCW7.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBDCW7.Controllers;

[ApiController]
[Route("api/[controller]")]  // api/trips
public class TripsController(IDbService dbService) : ControllerBase
{
    
    // zwraca wycieczki z ich podstawowymi informacjami
    [HttpGet]
    public async Task<IActionResult> GetAllTripsAsync()
    {
        return Ok(await dbService.GetAllTripsAsync());
    }
}