using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VillanonoRepositoryController : ControllerBase
{
    readonly IVillanonoRepository villanonoRepository;
    readonly IVillanonoLoadService villanonoLoadService;

    public VillanonoRepositoryController(
        IVillanonoRepository villanonoRepository,
        IVillanonoLoadService villanonoLoadService
    )
    {
        this.villanonoRepository = villanonoRepository;
        this.villanonoLoadService = villanonoLoadService;
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

    [HttpPost("BuySell")]
    public async Task<IActionResult> PutBuySellData(IFormFile csvFile)
    {
        if (csvFile == null || csvFile.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var stream = csvFile.OpenReadStream();
        await villanonoLoadService.BulkInsert<BuySellModel>(stream);

        return Ok();
    }
}
