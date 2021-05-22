using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.Domain.Models;
using OnlineRetailer.OrderApi.Command.Facade;
using OnlineRetailer.OrderApi.Query.Facade;

namespace OnlineRetailer.OrderApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderCommand _orderCommand;
        private readonly IOrderQuery _orderQuery;

        public OrderController(ILogger<OrderController> logger, IOrderCommand orderCommand, IOrderQuery orderQuery)
        {
            _logger = logger;
            _orderCommand = orderCommand;
            _orderQuery = orderQuery;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            _logger.Log(LogLevel.Debug, $"Get Request received for Customer: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var orderProjector = await _orderQuery.ByIdAsync(guid);
                _logger.Log(LogLevel.Debug, $"Customer with an id of {id}, was fetched");
                return Ok(new
                {
                    RawOrder = orderProjector.Projection,
                    OrderStatus = orderProjector.Projection.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Projection.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Projection.StockValidationStatus.ToString()
                });
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.Log(LogLevel.Debug, "GetAll request received");
            try
            {
                var orderProjectors = await _orderQuery.AllAsync();
                _logger.Log(LogLevel.Debug, "All Customer was fetched");
                return Ok(orderProjectors.Select(orderProjector => new
                {
                    RawOrder = orderProjector.Projection,
                    OrderStatus = orderProjector.Projection.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Projection.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Projection.StockValidationStatus.ToString(),
                    events = orderProjector.Stream.EventStream
                }));
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        [HttpGet("[action]/{customerId}")]
        public async Task<IActionResult> ByCustomer(string customerId)
        {
            _logger.Log(LogLevel.Debug, "GetByCustomer request received");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {customerId} to guid");
                var guid = Guid.Parse(customerId);
                var orderProjectors = await _orderQuery.ByCustomerAsync(guid);
                _logger.Log(LogLevel.Debug, $"All Orders of Customer {customerId} was fetched");
                return Ok(orderProjectors.Select(orderProjector => new
                {
                    RawOrder = orderProjector.Projection,
                    OrderStatus = orderProjector.Projection.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Projection.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Projection.StockValidationStatus.ToString()
                }));
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
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
    }

    public record NewOrderDto(IList<OrderLine> OrderLines, Guid CustomerId);
}