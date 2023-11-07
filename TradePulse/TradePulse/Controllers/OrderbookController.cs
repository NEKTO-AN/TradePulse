using Application.Orderbook.Read;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TradePulse.Controllers;

[Route("api/[controller]")]
public class OrderbookController : Controller
{
    private readonly ISender _sender;

    public OrderbookController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetOrderbookDataAsync(string symbol, long from, int count)
    {
        ReadOrderbookCommand command = new(symbol, from, count);

        ReadOrderbookResponse response = await _sender.Send(command);

        return Ok(response);
    }
}
