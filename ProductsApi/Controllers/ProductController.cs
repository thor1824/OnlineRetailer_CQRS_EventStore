using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.ProductsApi.Command.Facades;
using OnlineRetailer.ProductsApi.Query.Facades;

namespace OnlineRetailer.ProductsApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductCommand _productCommand;
        private readonly IProductQuery _productQuery;

        public ProductController(IProductQuery productQuery, IProductCommand productCommand,
            ILogger<ProductController> logger)
        {
            _productQuery = productQuery;
            _productCommand = productCommand;
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
                var product = await _productQuery.ByIdAsync(guid);
                _logger.Log(LogLevel.Debug, $"product with an id of {id}, was fetched");
                return Ok(new
                {
                    Product = product.Aggregate,
                    Events = product.Stream.EventStream
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
        ///     Fetches All Products
        /// </summary>
        /// <returns>An array og all products</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.Log(LogLevel.Debug, "Get request received");
            try
            {
                var products = await _productQuery.AllAsync();
                _logger.Log(LogLevel.Debug, "All product was fetched");
                return Ok(products.Select(product => new
                {
                    Product = product.Aggregate,
                    Events = product.Stream.EventStream
                }));
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
                var (wasSuccess, message) = await _productCommand.RemoveAsync(guid);
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
        ///     creates a product and assigns it with and id
        /// </summary>
        /// <param name="newDto"></param>
        /// <returns>the created product</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewProductDto newDto)
        {
            _logger.Log(LogLevel.Debug, "Post Request Received");
            try
            {
                var (wasSuccessAdded, messageAdded, id) = await _productCommand.AddAsync(newDto.Name,
                    newDto.Category, newDto.Price, newDto.ItemsInStock);
                if (!wasSuccessAdded)
                {
                    _logger.Log(LogLevel.Debug, $"Product was not added because: {messageAdded}");
                    return BadRequest(messageAdded);
                }

                _logger.Log(LogLevel.Debug, "Product was successfully created");

                return Created(new Uri($"{Request.Path}/{id.ToString()}", UriKind.Relative), newDto);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.ToString());
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        ///     Update a Products Name
        /// </summary>
        /// <param name="id">Id of the Product to Change</param>
        /// <param name="change">Object containing the new value of Name</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Name(string id, [FromBody] NameChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for Product: {id}/Name");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _productCommand.RenameAsync(guid, change.Name);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Error, $"Command was not applied. Reason: {message}");
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

        /// <summary>
        ///     Update a Products Category
        /// </summary>
        /// <param name="id">Id of the Product to Change</param>
        /// <param name="change">Object containing the new value of Category</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Category(string id, [FromBody] CategoryChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for Product: {id}/Category");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _productCommand.ChangeCategoryAsync(guid, change.Category);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Error, $"Command was not applied. Reason: {message}");
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

        /// <summary>
        ///     Update a Products Price
        /// </summary>
        /// <param name="id">Id of the Product to Change</param>
        /// <param name="change">Object containing the new value of Price</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Price(string id, [FromBody] PriceChangeDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for Product: {id}/Price");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _productCommand.ChangePriceAsync(guid, change.Price);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Error, $"Command was not applied. Reason: {message}");
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

        /// <summary>
        ///     Update a Products Stock
        /// </summary>
        /// <param name="id">Id of the Product to Change</param>
        /// <param name="change">Object containing the new value of Category</param>
        /// <returns></returns>
        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> IncreaseStock(string id, [FromBody] RestockDto change)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for Product: {id}/Stock");
            try
            {
                _logger.Log(LogLevel.Debug, $"parsing {id} to guid");
                var guid = Guid.Parse(id);
                var (wasSuccess, message) = await _productCommand.IncreaseStockAsync(guid, change.StockIncrease);
                if (!wasSuccess)
                {
                    _logger.Log(LogLevel.Error, $"Command was not applied. Reason: {message}");
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

    public record NewProductDto(string Name, string Category, decimal Price, int ItemsInStock);

    public record NameChangeDto(string Name);

    public record CategoryChangeDto(string Category);

    public record PriceChangeDto(decimal Price);

    public record RestockDto(int StockIncrease);


    public class PutProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int? IncreaseStockBy { get; set; }
        public int? ItemsToReserve { get; set; }
    }
}