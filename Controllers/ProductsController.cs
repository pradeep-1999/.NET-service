using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using System.Web;
//using System.Web.Http;
using System.Net.Http;
using System.Net;
using WebApplication2.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        private readonly EMailServices _eMailServices;
        public ProductsController(ProductService productService, EMailServices eMailServices)
        {
            _productService = productService;
            _eMailServices = eMailServices;
        }

        [HttpGet]
        public ActionResult<List<Product>> Get()
        {

            // Line no 36 has been commented because i have added timer functionality in EMailServices service. 
            // _eMailServices.fun1();
                      
            // EMailServices e1 = new EMailServices();

            return _productService.Get();

           
        }

        [HttpGet("{id:length(24)}"  , Name = "GetProduct")]
        public ActionResult<Product> Get(string id)
        {
            var product = _productService.Get(id);

            if (product == null)
            {
                return NotFound();
            }


            return product;
        }

        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            _productService.Create(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product);
        }

        [HttpPut("{id:length(24)}")]
        public Product Update(string id, Product productIn)
        {
            var product = _productService.Get(id);

            if (product == null)
            {
                return null;
            }


            if (!_eMailServices.check_credentials(productIn) || !_eMailServices.IsValidEmail(productIn.Email_id))

            {
                Console.WriteLine("Check your credentials\n");
                return null;
            }
          
            
            if (productIn.Id == null)
                productIn.Id = id;

            _productService.Update(id, productIn);

            return productIn;
        }


        [HttpDelete("{id:length(24)}")]
        public Product Delete(string id)
        {
            var product = _productService.Get(id);

            if (product == null)
            {
                return null;
            }

            var product_deleted= _productService.Get(id);
            
            _productService.Remove(product.Id);

            return product_deleted ;
        }


    }
}
