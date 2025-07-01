using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ReturnController : ControllerBase
{
    private readonly IReturnService _returnService;

    public ReturnController(IReturnService returnService)
    {
        _returnService = returnService;
    }

    [HttpPost]
    public async Task<IActionResult> ReturnProduct([FromBody] ReturnProductDTO returnProductDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _returnService.ProcessReturnAsync(returnProductDto);
        if (!result)
            return NotFound("Produto não encontrado");

        return Ok("Devolução processada com sucesso");
    }
}