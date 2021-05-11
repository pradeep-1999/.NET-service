using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using MongoDB.Driver;


namespace WebApplication2.Services
{
    public class ProductService
    {
        public readonly IMongoCollection<Product> _products;

        public ProductService(IProductDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _products = database.GetCollection<Product>(settings.ProductCollectionName);
        
        }

        public List<Product> Get() =>
       _products.Find(Product => true).ToList();

        public Product Get(string id) =>
            _products.Find<Product>(x => x.Id == id).FirstOrDefault();

        public Product Create(Product product)
        {
            _products.InsertOne(product);
            return product;
        }

       public void Update(string id, Product productIn) =>
            _products.ReplaceOne(product => product.Id == id, productIn);                       //product => product.Id == id,


        public void Remove(Product productIn) =>
            _products.DeleteOne(product => product.Id == productIn.Id);

        public void Remove(string id) =>
            _products.DeleteOne(product => product.Id == id);   

    }
}
