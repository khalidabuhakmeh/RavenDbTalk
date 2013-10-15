using System;
using System.Collections.Generic;
using System.Linq;

namespace RavenDbTalk.Tests.Models
{
    public class Example
    {
        public string Id { get; set; }
        public string Manufacturer { get; set; }
        public DateTime Date { get; set; }
        public decimal Cost { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public IList<string> Tags { get; set; }
    }

    public class Order
    {
        public Order()
        {
            Items = new List<OrderLineItem>();
        }

        public string Id { get; set; }
        public IList<OrderLineItem> Items { get; set; }

        public void Add(Product product, int quantity)
        {
            var existing = Items.FirstOrDefault(x => x.ProductId == product.Id);

            if (existing == null)
                Items.Add((existing = new OrderLineItem()));

            if (quantity <= 0)
                Items.Remove(existing);

            existing.ProductId = product.Id;
            existing.ProductName = product.Name;
            existing.Price = product.Price;
        }
    }

    public class OrderLineItem
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public string ProductName { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public static class Helper
    {
        public static Random Random = new Random();

        public static IList<Example> GetExamples(int count)
        {
            return Enumerable.Range(1, count)
                 .Select(i =>
                 {
                     var example = new Example
                                   {
                                       Cost = Random.Next(1, 100),
                                       Date = DateTime.Now.AddDays(Random.Next(1, 100)),
                                       Manufacturer = string.Format("Company #{0}", Random.Next(1, 10)),
                                       Price = Random.Next(1, 100),
                                       Quantity = Random.Next(1, 100),
                                       Tags = new[] {
                                            "product",
                                            string.Format("tag #{0}", Random.Next(1, 5))
                                        }.ToList()
                                   };

                     example.Tags.Add(example.Manufacturer);
                     return example;
                 }).ToList();
        }
    }
}