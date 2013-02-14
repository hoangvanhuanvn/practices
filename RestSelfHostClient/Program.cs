using System;
using System.Collections.Generic;
using System.Net.Http;
using RestSelfHostDomain;

namespace RestSelfHostClient
{
    class Program
    {
        static readonly HttpClient Client = new HttpClient();

        static void Main(string[] args)
        {
            Client.BaseAddress = new Uri("http://localhost:8080");

            ListAllProducts();
            ListProduct(1);
            ListProducts("toys");

            Console.WriteLine("Press Enter to quit.");
            Console.ReadLine();
        }

        static void ListAllProducts()
        {
            HttpResponseMessage resp = Client.GetAsync("api/products").Result;
            resp.EnsureSuccessStatusCode();

            var products = resp.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            foreach (var p in products)
            {
                Console.WriteLine("{0} {1} {2} ({3})", p.Id, p.Name, p.Price, p.Category);
            }
        }

        static void ListProduct(int id)
        {
            var resp = Client.GetAsync(string.Format("api/products/{0}", id)).Result;
            resp.EnsureSuccessStatusCode();

            var product = resp.Content.ReadAsAsync<Product>().Result;
            Console.WriteLine("ID {0}: {1}", id, product.Name);
        }

        static void ListProducts(string category)
        {
            Console.WriteLine("Products in '{0}':", category);

            string query = string.Format("api/products?category={0}", category);

            var resp = Client.GetAsync(query).Result;
            resp.EnsureSuccessStatusCode();

            var products = resp.Content.ReadAsAsync<IEnumerable<Product>>().Result;
            foreach (var product in products)
            {
                Console.WriteLine(product.Name);
            }
        }
    }
}
