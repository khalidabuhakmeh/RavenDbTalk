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
        public decimal Price { get; set; }
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

    public class Dog
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }

        public void Woof()
        {
            Console.WriteLine("Woof, Woof, Woof, Squirrel!");
        }
    }

    public class Cat
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }

        public void Meooow()
        {
            Console.WriteLine("Meeeeeeeeeeow");
        }
    }

    public class FamousQuote
    {
        public string Id { get; set; }
        public string By { get; set; }
        public string Said { get; set; }

        public override string ToString()
        {
            return string.Format("{0} said : \"{1}\"", By, Said);
        }
    }
}