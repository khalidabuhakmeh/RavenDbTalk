using System;
using System.Collections.Generic;
using System.Linq;

namespace RavenDbTalk.Tests.Models
{
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