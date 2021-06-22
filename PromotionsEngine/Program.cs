using System;
using System.Collections.Generic;
using System.Linq;

namespace PromotionsEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<Promotion> promotions = new List<Promotion>()
            {
                new Promotion (1, new Dictionary<string, int>{ { "A", 3 } }, 130),
                new Promotion(2, new Dictionary<string, int>{ { "B", 2 } }, 45),
                new Promotion(3, new Dictionary<string, int>{ { "C", 1 },{ "D", 1 } }, 30)
            };

            // Prepare orders
            List<Order> orders = new List<Order>();
            Order order1 = new Order(1, new List<Product>() { new Product("A"), new Product("A"), new Product("B"), new Product("B"), new Product("C"), new Product("D") });
            Order order2 = new Order(2, new List<Product>() { new Product("A"), new Product("A"), new Product("A"), new Product("A"), new Product("A"), new Product("A"), new Product("B") });
            Order order3 = new Order(3, new List<Product>() { new Product("A"), new Product("A"), new Product("D"), new Product("B"), new Product("B") });
            orders.AddRange(new Order[] { order1, order2, order3 });
            
            // Check if order meets promotion
            foreach (Order ord in orders)
            {
                List<decimal> promotionalPrices = promotions
                    .Select(promo => PromotionService.GetTotalPriceByPromotion(ord, promo))
                    .ToList();
                decimal originalPrice = ord.Products.Sum(x => x.Price);
                decimal promotionalPrice = promotionalPrices.Sum();
                Console.WriteLine($"OrderID: {ord.OrderId} => Original price: {originalPrice.ToString("0.00")} | Discount: {promotionalPrice.ToString("0.00")} | Final price: {(originalPrice - promotionalPrice).ToString("0.00")}");
            }
        }
    }



    public static class PromotionService
    {
        public static decimal GetTotalPriceByPromotion(Order order, Promotion promotion)
        {
            decimal totalPrice = 0M;

            // Get count of promoted products in order
            int countOfPromotedProductsInTheOrder = order.Products
                .GroupBy(x => x.Id)
                .Where(grp => promotion.ProductInfo.Any(y => grp.Key == y.Key && grp.Count() >= y.Value))
                .Select(grp => grp.Count())
                .Sum();


            // Get count of promoted products in the promotion
            int countOfPromotedProductsInThePromotion = promotion.ProductInfo.Sum(kvp => kvp.Value);

            while (countOfPromotedProductsInTheOrder >= countOfPromotedProductsInThePromotion)
            {
                totalPrice += promotion.PromoPrice;
                countOfPromotedProductsInTheOrder -= countOfPromotedProductsInThePromotion;
            }

            return totalPrice;
        }
    }

    public class Product
    {
        public string Id { get; set; }
        public decimal Price { get; set; }


        public Product(string id)
        {
            this.Id = id;
            switch (id)
            {
                case "A":
                    this.Price = 50m;

                    break;
                case "B":
                    this.Price = 30m;

                    break;
                case "C":
                    this.Price = 20m;

                    break;
                case "D":
                    this.Price = 15m;
                    break;
            }
        }
    }

    public class Promotion
    {
        public int PromotionId { get; set; }
        public Dictionary<string, int> ProductInfo { get; set; }
        public decimal PromoPrice { get; set; }

        public Promotion(int promotionId, Dictionary<string, int> productInfo, decimal promoPrice)
        {
            this.PromotionId = promotionId;
            this.ProductInfo = productInfo;
            this.PromoPrice = promoPrice;
        }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public List<Product> Products { get; set; }

        public Order(int orderId, List<Product> products)
        {
            this.OrderId = orderId;
            this.Products = products;
        }
    }



}
