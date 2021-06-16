using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.OrderQueryApi.Query.Facade;

namespace OnlineRetailer.OrderQueryApi.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderQuery _orderQuery;

        public OrderController(ILogger<OrderController> logger, IOrderQuery orderQuery)
        {
            _logger = logger;
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
                    RawOrder = orderProjector.Aggregate,
                    OrderStatus = orderProjector.Aggregate.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Aggregate.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Aggregate.StockValidationStatus.ToString()
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
                    RawOrder = orderProjector.Aggregate,
                    OrderStatus = orderProjector.Aggregate.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Aggregate.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Aggregate.StockValidationStatus.ToString(),
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
                    RawOrder = orderProjector.Aggregate,
                    OrderStatus = orderProjector.Aggregate.OrderStatus.ToString(),
                    CustomerValidationStatus = orderProjector.Aggregate.CustomerValidationStatus.ToString(),
                    StockValidationStatus = orderProjector.Aggregate.StockValidationStatus.ToString()
                }));
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }
    }
}