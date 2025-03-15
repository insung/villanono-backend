using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VillanonoRepositoryController : ControllerBase
{
    readonly IVillanonoRepository villanonoRepository;

    public VillanonoRepositoryController(IVillanonoRepository villanonoRepository)
    {
        this.villanonoRepository = villanonoRepository;
    }

    [HttpGet("Ping")]
    public async Task<IActionResult> Ping()
    {
        await villanonoRepository.Ping();
        return Ok();
    }

    [HttpPost("CreateDefaultIndex")]
    public async Task<IActionResult> CreateDefaultIndex()
    {
        await villanonoRepository.CreateDefaultDatabase();
        return Ok();
    }
}
