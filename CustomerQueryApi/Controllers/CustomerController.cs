using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.CustomerQueryApi.Query.Facade;

namespace OnlineRetailer.CustomerQueryApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerQuery _customerQuery;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerQuery customerQuery,
            ILogger<CustomerController> logger)
        {
            _customerQuery = customerQuery;
            _logger = logger;
        }

        /// <summary>
        ///     Fetches a Customer by the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a Customer, with the given id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.Log(LogLevel.Debug, $"Get Request received for Customer: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var customer = await _customerQuery.ByIdAsync(guid);
                _logger.Log(LogLevel.Debug, $"Customer with an id of {id}, was fetched");
                return Ok(new
                {
                    Customer = customer.Aggregate,
                    Events = customer.Stream.EventStream
                });
            }
            catch (FormatException fe)
            {
                _logger.Log(LogLevel.Error, "Formatting of guid failed, Not Valid Guid");
                return BadRequest(fe.Message);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Fetches All Customers
        /// </summary>
        /// <returns>An array og all Customers</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.Log(LogLevel.Debug, "Get request received");
            try
            {
                var customers = await _customerQuery.AllAsync();
                _logger.Log(LogLevel.Debug, "All Customer was fetched");
                return Ok(customers.Select(customer => new
                {
                    Customer = customer.Aggregate,
                    Events = customer.Stream.EventStream
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