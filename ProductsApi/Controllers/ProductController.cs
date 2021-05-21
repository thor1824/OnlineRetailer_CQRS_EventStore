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
                var products = await _productQuery.AllAsync();
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
                var (wasSuccessAdded, messageAdded, id) = await _productCommand.AddAsync(postDto.Name,
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
        ///     Update a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] PutProductDto product)
        {
            _logger.Log(LogLevel.Debug, $"Put Request Received for product: {id}");
            try
            {
                var guid = Guid.Parse(id);
                if (product.Name != null)
                {
                    _logger.Log(LogLevel.Debug, $"Product: {id}, Change Name");
                    await _productCommand.RenameAsync(guid, product.Name);
                }

                if (product.Category != null)
                {
                    _logger.Log(LogLevel.Debug, $"Product: {id}, Change Category");
                    await _productCommand.ChangeCategoryAsync(guid, product.Category);
                }

                if (product.Price.HasValue)
                {
                    _logger.Log(LogLevel.Debug, $"Product: {id}, Change Price");
                    await _productCommand.ChangePriceAsync(guid, product.Price.Value);
                }

                if (product.IncreaseStockBy.HasValue)
                {
                    _logger.Log(LogLevel.Debug, $"Product: {id}, Increase stock");
                    await _productCommand.IncreaseStuckAsync(guid, product.IncreaseStockBy.Value);
                }

                if (product.ItemsToReserve.HasValue)
                {
                    _logger.Log(LogLevel.Debug, $"Product: {id}, Reserve amount");
                    await _productCommand.ReserveStuckAsync(guid, product.ItemsToReserve.Value);
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
    }

    public class PostProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int ItemsInStock { get; set; }
    }

    public class PutProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public int? IncreaseStockBy { get; set; }
        public int? ItemsToReserve { get; set; }
    }
}