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

        public static IList<FamousQuote> GetQuotes()
        {
            return new List<FamousQuote>
            {
                new FamousQuote { By = "Dhirubhai Ambani", Said = "If you don’t build your dream, someone else will hire you to help them build theirs."},
                new FamousQuote { By = "David Allen", Said = "You can do anything, but not everything."},
                new FamousQuote { By = "Unkown Author", Said = "The richest man is not he who has the most, but he who needs the least" },
                new FamousQuote { By = "Wayne Gretzky", Said = "You miss 100 percent of the shots you never take."},
                new FamousQuote { By = "Ambrose Redmoon", Said = "Courage is not the absence of fear, but rather the judgement that something else is more important than fear"},
                new FamousQuote { By = "Ghandi", Said = "You must be the change you wish to see in the world."},
                new FamousQuote { By = "Lin-Chi", Said = "When hungry, eat your rice; when tired, close your eyes. Fools may laugh at me, but wise men will know what I mean."},
                new FamousQuote { By = "Artistotle", Said = "We are what we repeatedly do; excellence, then, is not an act but a habit."},
                new FamousQuote { By = "Artistotle", Said = "It is the mark of an educated mind to be able to entertain a thought without accepting it."},
                new FamousQuote { By = "Baltasar Gracian", Said = "A wise man gets more use from his enemies than a fool from his friends."},
                new FamousQuote { By = "Abraham Lincoln", Said = "Better to remain silent and be thought a fool than to speak out and remove all doubt." },
                new FamousQuote { By = "Walt Disney", Said = "All our dreams can come true, if we have the courage to pursue them." },
                new FamousQuote { By = "James Dean", Said = "Dream as if you'll live forever. Live as if you'll die today." },
                new FamousQuote { By = "Oscar Levant", Said = "What the world needs is more geniuses with humility, there are so few of us left."},
                new FamousQuote { By = "Cullen Hightower", Said = "Laughing at our mistakes can lengthen our own life. Laughing at someone else’s can shorten it."},
                new FamousQuote { By = "Carl Sagan", Said = "Just the fact that some geniuses were laughed at does not imply that all who are laughed at are geniuses. They laughed at Columbus, they laughed at Fulton, they laughed at the Wright brothers. But they also laughed at Bozo the Clown."},
                // not famous quotes about apples
                new FamousQuote { By = "Johnny apple apple", Said = "I I really really love love the apple apple apple apple apple they they are are the best best."},
                new FamousQuote { By = "Johnny apple apple", Said = "I I really really love love the apple apple apple apple apple they they are are the best best."},
                new FamousQuote { By = "Johnny apple apple", Said = "I I really really love love the apple apple apple apple apple they they are are the best best."},
                new FamousQuote { Id = "famousquotes/not", By = "Johnny apple apple", Said = "I I really really love love the apple apple apple apple apple they they are are the best best."},
            };
        }
    }
}