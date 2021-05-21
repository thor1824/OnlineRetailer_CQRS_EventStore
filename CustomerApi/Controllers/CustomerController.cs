using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.CustomerApi.Command.Facade;
using OnlineRetailer.CustomerApi.Query.Facade;

namespace OnlineRetailer.CustomerApi.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerCommand _customerCommand;
        private readonly ICustomerQuery _customerQuery;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerQuery customerQuery, ICustomerCommand customerCommand,
            ILogger<CustomerController> logger)
        {
            _customerQuery = customerQuery;
            _customerCommand = customerCommand;
            _logger = logger;
        }

        /// <summary>
        ///     Fetches a product by the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a product, with the given id</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            _logger.Log(LogLevel.Debug, $"Get Request received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var product = await _customerQuery.ByIdAsync(guid);
                _logger.Log(LogLevel.Debug, $"product with an id of {id}, was fetched");
                return Ok(product.Projection);
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
        ///     Fetches All Products
        /// </summary>
        /// <returns>An array og all products</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.Log(LogLevel.Debug, "Get request received");
            try
            {
                var products = await _customerQuery.AllAsync();
                _logger.Log(LogLevel.Debug, "All product was fetched");
                return Ok(products.Select(product => product.Projection));
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     creates a product and assigns it with and id
        /// </summary>
        /// <param name="postDto"></param>
        /// <returns>the created product</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostProductDto postDto)
        {
            _logger.Log(LogLevel.Debug, "Post Request Received");
            try
            {
                var (wasSuccessAdded, messageAdded, id) = await _customerCommand.AddAsync(postDto.Name,
                    postDto.Category, postDto.Price, postDto.ItemsInStock);
                if (!wasSuccessAdded)
                {
                    _logger.Log(LogLevel.Debug, $"Product was not added because: {messageAdded}");
                    return BadRequest(messageAdded);
                }

                _logger.Log(LogLevel.Debug, "Product was successfully created");

                return Created(new Uri($"{Request.Path}/{id.ToString()}", UriKind.Relative), postDto);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Deletes a product with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A NoContent Response</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.Log(LogLevel.Debug, $"Delete Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _customerCommand.RemoveAsync(guid);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Debug, $"Product was not deleted because: {message}");
                    return BadRequest(message);
                }

                _logger.Log(LogLevel.Debug, "Product was successfully Deleted");
                return NoContent();
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
        ///     Update a Customers Name
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of Name</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Name(string id, [FromBody] NameChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _customerCommand.ChangeName(guid, change.NewName);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Update a Customers EmailAddress
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of Email</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Email(string id, [FromBody] EmailChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _customerCommand.ChangeEmail(guid, change.NewEmail);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Update a Customers Phone number
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of Phone</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Phone(string id, [FromBody] PhoneChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _customerCommand.ChangePhone(guid, change.NewPhone);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Update a Customers BillingAddress
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of BillingAddress</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> BillingAddress(string id, [FromBody] BillingAddressChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _customerCommand.ChangeBillingAddress(guid, change.NewBillingAddress);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Update a Customers ShippingAddress
        /// </summary>
        /// <param name="id">Id of the customer to Change</param>
        /// <param name="change">Object containing the new value of ShippingAddress</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> ShippingAddress(string id, [FromBody] ShippingAddressChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) =
                    await _customerCommand.ChangeShippingAddress(guid, change.NewShippingAddress);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                return BadRequest(e.Message);
            }
        }
    }

    public class PostProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int ItemsInStock { get; set; }
    }

    public record NameChangeDto(string NewName);

    public record EmailChangeDto(string NewEmail);

    public record PhoneChangeDto(string NewPhone);

    public record BillingAddressChangeDto(string NewBillingAddress);

    public record ShippingAddressChangeDto(string NewShippingAddress);

    public class PutProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int? IncreaseStockBy { get; set; }
        public int? ItemsToReserve { get; set; }
    }
}