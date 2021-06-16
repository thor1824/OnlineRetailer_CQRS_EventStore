using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineRetailer.ProductQueryApi.Query.Facades;

namespace OnlineRetailer.ProductQueryApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductQuery _productQuery;

        public ProductController(IProductQuery productQuery, ILogger<ProductController> logger)
        {
            _productQuery = productQuery;
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
    }
}