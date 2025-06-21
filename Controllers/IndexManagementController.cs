using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IndexManagementController : ControllerBase
{
    readonly IIndexManagementRepository indexManagementRepository;

    public IndexManagementController(IIndexManagementRepository indexManagementRepository)
    {
        this.indexManagementRepository = indexManagementRepository;
    }

    /// <summary>
    /// 살아있는지 확인
    /// </summary>
    /// <returns></returns>
    [HttpGet("Ping")]
    public async Task<IActionResult> Ping()
    {
        await indexManagementRepository.Ping();
        return Ok();
    }

    /// <summary>
    /// 인덱스 생성
    /// </summary>
    /// <param name="indexFullName"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    [HttpPost("{indexFullName}/{dataType}")]
    public async Task<IActionResult> CreateIndex(string indexFullName, VillanonoDataType dataType)
    {
        if (dataType == VillanonoDataType.BuySell)
            await indexManagementRepository.CreateDataIndex<BuySellModel>(indexFullName);
        else if (dataType == VillanonoDataType.Rent)
            await indexManagementRepository.CreateDataIndex<RentModel>(indexFullName);
        return Ok();
    }

    /// <summary>
    /// 인덱스 삭제
    /// </summary>
    /// <param name="indexFullName"></param>
    /// <returns></returns>
    [HttpDelete("{indexFullName}")]
    public async Task<IActionResult> DeleteIndex(string indexFullName)
    {
        await indexManagementRepository.DeleteIndex(indexFullName);
        return Ok();
    }

    /// <summary>
    /// 인덱스 생성
    /// </summary>
    /// <param name="indexFullName"></param>
    /// <returns></returns>
    [HttpPost("{indexFullName}")]
    public async Task<IActionResult> CreateIndex(string indexFullName)
    {
        var response = await indexManagementRepository.CreateIndex(indexFullName);
        return Ok(response);
    }

    /// <summary>
    /// 인덱스 재구성
    /// </summary>
    /// <param name="sourceIndexName"></param>
    /// <param name="targetIndexName"></param>
    /// <returns></returns>
    [HttpPost("ReIndex/{sourceIndexName}/{targetIndexName}")]
    public async Task<IActionResult> ReIndex(string sourceIndexName, string targetIndexName)
    {
        await indexManagementRepository.ReIndex(sourceIndexName, targetIndexName);
        return Ok();
    }

    [HttpPost("DefaultTemplate")]
    public async Task<IActionResult> CreateTemplate()
    {
        await indexManagementRepository.CreateDefaultIndexTemplate();
        return Ok("Default index template created successfully.");
    }

    [HttpGet("DefaultTemplate")]
    public async Task<IActionResult> GetDefaultTemplate()
    {
        var response = await indexManagementRepository.GetDefaultIndexTemplate();
        var jsonObject = JsonSerializer.Deserialize<object>(response.ApiCall.ResponseBodyInBytes);
        return Ok(jsonObject);
    }

    [HttpGet("Mapping/{indexName}")]
    public async Task<IActionResult> GetIndexMapping(string indexName)
    {
        var response = await indexManagementRepository.GetIndexMapping(indexName);
        var jsonObject = JsonSerializer.Deserialize<object>(response.ApiCall.ResponseBodyInBytes);
        return Ok(jsonObject);
    }
}
