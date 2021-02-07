using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Invoice
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Dictionary<string, Product> productMap = p.GetProductMap();
            IEnumerable<string> inputFile = File.ReadLines("Input.txt");
            
            Console.WriteLine(string.Format("{0} | {1} | {2} | {3}","NAME".PadRight(30),"QTY".PadRight(10),"UNIT_COST".PadRight(10),"COST"));

            double subTotal = 0, vat = 0, additionalTax = 0;
            foreach (string line in inputFile)
            {
                string quantity = Regex.Match(line, @"^\d+").ToString();
                var product = line.Substring(quantity.Length).Split('@');
                string productName = product[0].Trim();
                string unitPrice = product[1].Trim();
                double price = Convert.ToInt32(quantity) * Convert.ToDouble(unitPrice);

                subTotal += price;
                int productTypeId = productMap.ContainsKey(productName) ? productMap[productName].ProductTypeId : (int)ProductType.Others;
                if (productTypeId != (int)ProductType.Food && productTypeId != (int)ProductType.Toy && productTypeId != (int)ProductType.Medicine)
                {
                    vat += price * 0.125;
                }
                bool isImported = productMap.ContainsKey(productName) ? productMap[productName].IsImported : false;
                if (isImported)
                {
                    additionalTax += (price + price * 0.125) * 0.024;
                }
                string print = string.Format("{0} | {1} | {2} | {3}", productName.PadRight(30), quantity.PadRight(10), unitPrice.PadRight(10), price);
                Console.WriteLine(print);
            }
            Console.WriteLine(string.Format("SubTotal: {0}", subTotal));
            Console.WriteLine(string.Format("Value Added Tax: {0}", vat.ToString("0.##")));
            Console.WriteLine(string.Format("Additional Tax: {0}", additionalTax.ToString("0.##")));
            Console.WriteLine(string.Format("Total: {0}", (subTotal + vat + additionalTax).ToString("0.##")));
        }

        List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>()
            {
                new Product {Id=1,Name="soap", ProductTypeId = 4, IsImported = false},
                new Product {Id=2,Name="potato chips packet", ProductTypeId = 1,  IsImported = false},
                new Product {Id=3,Name="music CD", ProductTypeId = 4, IsImported = false},
                new Product {Id=4,Name="imported bottle of perfume", ProductTypeId = 4, IsImported = true},
                new Product {Id=5,Name="packet of crocin", ProductTypeId = 3, IsImported = false},
            };

            return products;
        }

        Dictionary<string,Product> GetProductMap()
        {
            Program p = new Program();
            Dictionary<string, Product> productMap = new Dictionary<string, Product>();
            List<Product> products = GetAllProducts();
            foreach(Product product in products)
            {
                productMap.Add(product.Name, product);
            }
            return productMap;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public bool IsImported { get; set; }
    }

    public enum ProductType
    {
        Food = 1,
        Toy,
        Medicine,
        Others
    }
}
