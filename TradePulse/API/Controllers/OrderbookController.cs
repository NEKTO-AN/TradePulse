using Application.Orderbook.Read;
using Application.PumpDumpSnapshot.Read;
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

        [HttpGet("data")]
        public async Task<IActionResult> GetDataAsync(string symbol, long fromTs, int count)
        {
            ReadOrderbookCommand command = new(symbol, fromTs, count);
            
            ReadOrderbookResponse response = await _sender.Send(command);
            return Ok(response);
        }

        [HttpGet("anomaly-zones")]
        public async Task<IActionResult> GetAnomalyZonesAsync(string symbol, long fromTs, int count)
        {
            ReadPumpDumpSnapshotCommand command = new(symbol, fromTs, count);

            ReadPumpDumpSnapshotResponse response = await _sender.Send(command);
            return Ok(response);
        }
    }
}