using Application.Orderbook.Read;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderbookController : ControllerBase
    {
        private readonly ISender _sender;

        public OrderbookController(ISender sender)
        {
            _sender = sender;
        }


        [HttpGet("ranges-collected-data")]
        public async Task<IActionResult> GetRangesCollectedDataAsync(string symbol, int page)
        {
            return Ok();
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetDataAsync(string symbol, long fromTs, long toTs)
        {
            ReadOrderbookCommand command = new(symbol, fromTs, /*todo change it later*/ 50);
            
            ReadOrderbookResponse response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpGet("ranges-collected-data")]
        public async Task<IActionResult> GetAnomalyZonesAsync(string symbol, int page)
        {
            return Ok();
        }
    }
}