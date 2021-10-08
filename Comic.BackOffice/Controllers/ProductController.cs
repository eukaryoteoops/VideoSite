using System.Collections.Generic;
using System.Threading.Tasks;
using Comic.BackOffice.Commands.Product;
using Comic.BackOffice.ReadModels.Product;
using Comic.Common.Utilities;
using Comic.Domain.Entities;
using Comic.Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Comic.BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductDefaultConfigsRepository _productDefaultConfigsRepository;

        public ProductController(IProductRepository productRepository, IProductDefaultConfigsRepository productDefaultConfigsRepository)
        {
            _productRepository = productRepository;
            _productDefaultConfigsRepository = productDefaultConfigsRepository;
        }

        [HttpGet]
        [SwaggerResponse(typeof(IEnumerable<ProductRM>))]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAsync(o => o.State != 2);
            return Ok(ResponseUtility.CreateSuccessResopnse(products.Adapt<IEnumerable<ProductRM>>()));
        }

        [HttpGet("default")]
        [SwaggerResponse(typeof(IEnumerable<ProductDefaultRM>))]
        public async Task<IActionResult> GetDefault()
        {
            var products = await _productDefaultConfigsRepository.GetAsync();
            return Ok(ResponseUtility.CreateSuccessResopnse(products.Adapt<IEnumerable<ProductDefaultRM>>()));
        }

        [HttpPatch]
        public async Task<IActionResult> AddProduct(AddProduct cmd)
        {
            var product = new Products(cmd.Name, cmd.Price, cmd.Type, cmd.Value);
            await _productRepository.AddAsync(product);
            return Ok();
        }

        [HttpPost("state")]
        public async Task<IActionResult> UpdateState(UpdateProductState cmd)
        {
            var product = await _productRepository.GetOneAsync(o => o.Id == cmd.Id);
            product.UpdateState(cmd.State);
            await _productRepository.UpdateAsync(product);
            return Ok();
        }

        [HttpPost("order")]
        public async Task<IActionResult> UpdateOrder(UpdateProductOrder cmd)
        {
            await _productRepository.UpdateProductOrder(cmd.ProductIds);
            return Ok();
        }

        [HttpPost("default")]
        public async Task<IActionResult> UpdateDefault(UpdateProductDefault cmd)
        {
            var productDefault = await _productDefaultConfigsRepository.GetOneAsync(o => o.Type == cmd.Type);
            productDefault.UpdateProductDefault(cmd.Id);
            await _productDefaultConfigsRepository.UpdateAsync(productDefault);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> ArchiveProduct(ArchiveProduct cmd)
        {
            var product = await _productRepository.GetOneAsync(o => o.Id == cmd.Id);
            product.ArchiveProdcut();
            await _productRepository.UpdateAsync(product);
            return Ok();
        }
    }
}
