﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Raven.Database.Linq;
using Raven.Database.Indexing;
using Newtonsoft.Json.Linq;

namespace Raven.Sample.EventSourcing
{
    [DisplayName("Aggregates/ShoppingCart")]
    public class ShoppingCartEventsToShopingCart : AbstractViewGenerator
    {
        public ShoppingCartEventsToShopingCart()
        {
            MapDefinition = docs => docs.Where(document => document.For == "ShoppingCart");
            GroupByExtraction = source => source.ShoppingCartId;
            ReduceDefinition = Reduce;

            Indexes.Add("Id", FieldIndexing.NotAnalyzed);
            Indexes.Add("Aggregate", FieldIndexing.No);
        }

        private static IEnumerable<object> Reduce(IEnumerable<dynamic> source)
        {
            foreach (var events in source
                .GroupBy(@event => @event.ShoppingCartId))
            {
                var cart = new ShoppingCart { Id = events.Key };
                foreach (var @event in events.OrderBy(x => x.Timestamp))
                {
                    switch ((string)@event.Type)
                    {
                        case "Create":
                            cart.Customer = new ShoppingCartCustomer
                            {
                                Id = @event.CustomerId,
                                Name = @event.CustomerName
                            };
                            break;
                        case "Add":
                            cart.AddToCart(@event.ProductId, @event.ProductName, (decimal)@event.Price);
                            break;
                        case "Remove":
                            cart.RemoveFromCart(@event.ProductId);
                            break;
                    }
                }
                yield return new
                {
                    cart.Id,
                    Aggregate = JObject.FromObject(cart)
                };
            }
        }
    }
}