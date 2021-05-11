using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Controllers;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using WebApplication2.Services;
using MailKit.Net.Smtp;
using MimeKit;
using MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Services
{
    public class EMailServices: IHostedService, IDisposable
    {
        private Timer _timer;

        public  IMongoCollection<Product> _products;

        public ProductService _productService;
        public EMailServices(IProductDatabaseSettings settings, ProductService productService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _products = database.GetCollection<Product>(settings.ProductCollectionName);

            _productService = productService;
        }


        //Because of this function no need to call 'Fun1()' explicitly from controller
        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(Fun1, null, TimeSpan.Zero, TimeSpan.FromSeconds(120));

            return Task.CompletedTask;
        }

        // To understand this function understand componentmodel.dataannotations header file .net....we could've also gone for regex
        public bool IsValidEmail(string source)
        {
            return new EmailAddressAttribute().IsValid(source);
        }

        public bool check_credentials(Product p1)
        {
            if (p1.Name.Length == 0 || p1.Category.Length == 0 || p1.Price <= 0 || p1.service_period <= 0 || p1.start_date == null)
                return false;
            
            return true;
        }

        public void Fun1(object state)
        {

            try
            {
                // var prod_1 = new Product();

                List<Product> my_list = _products.Find(x => true).ToListAsync().Result;

                foreach (Product p1 in my_list)
                {


                        TimeSpan diffDate = (DateTime.Now - p1.start_date);

                        int diff_date = Convert.ToInt32(diffDate.TotalDays);

                    if (check_credentials(p1)) { 

                        if (diff_date >= p1.service_period && IsValidEmail(p1.Email_id))
                        {
                            MimeMessage message = new MimeMessage();

                            MailboxAddress from = new MailboxAddress("PRADEEP", "yopradeep99@gmail.com");
                            message.From.Add(from);

                            // Changes have been made in trail line i.e email id is now NOT hardcoded

                            MailboxAddress to = new MailboxAddress("pradeep99", p1.Email_id);
                            message.To.Add(to);

                            message.Subject = "This is email subject for due product service of product " + p1.Name + "in next 3 days starting from date \n"+DateTime.Now;

                            SmtpClient client = new SmtpClient();
                            client.Connect("smtp.gmail.com", 465, true);
                            client.Authenticate("yopradeep99", "9136209045");
                       //     client.UseDefaultCredentials = true;

                          // client.Send(message);
                            client.Disconnect(true);
                             client.Dispose();

                          p1.start_date = DateTime.Now;

                        _productService.Update(p1.Id,p1);
                    
                    }

                        else
                        {
                            Console.WriteLine("Check ur credentials");

                        }
                    }
                

            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }


        // Not yet implemented
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Not yet implemented
        public void Dispose()
        {
            throw new NotImplementedException();
        }



    }
}
     


