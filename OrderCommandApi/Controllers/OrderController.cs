using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.OrderApi.Command.Facade;

namespace OnlineRetailer.OrderCommandApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderCommand _orderCommand;

        public OrderController(ILogger<OrderController> logger, IOrderCommand orderCommand)
        {
            _logger = logger;
            _orderCommand = orderCommand;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewOrderDto newOrderDto, CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Debug, "Post Request Received");
            try
            {
                var (wasSuccess, message, id) = await _orderCommand.PlaceAsync(newOrderDto.CustomerId,
                    newOrderDto.OrderLines, cancellationToken);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Debug, $"Order was not added because: {message}");
                    return BadRequest(message);
                }

                _logger.Log(LogLevel.Debug, "Order was successfully created");

                return Created(new Uri($"{Request.Path}/{id.ToString()}", UriKind.Relative), newOrderDto);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }
        
        /// <summary>
        ///     Update a Customers Name
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of Name</param>
        /// <returns></returns>
        [HttpPatch("{id}/[action]")]
        public async Task<IActionResult> Cancel(string id)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for Customer: {id}/Name");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _orderCommand.CancelAsync(guid);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Error, $"Command on {id} was not applied. Reason: {message}");
                    return BadRequest(message);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }
    }

    public record NewOrderDto(IList<OrderLine> OrderLines, Guid CustomerId);
}