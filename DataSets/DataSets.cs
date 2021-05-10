using OtomatUygulamasi.Models;
using System.Collections.Generic;
using System.Linq;

namespace OtomatUygulamasi.DataSets
{
    public class DataSets
    {
        //Ürünler
        //mail'de verilen dataset'e göre dolduruldu
        public static List<Products> ProductList()
        {
            List<Products> productList = new List<Products>();

            productList.Add(new Products { Id = 1, Name = "Dimes", Price = 4.3m });
            productList.Add(new Products { Id = 2, Name = "Coca Cola", Price = 8m });
            productList.Add(new Products { Id = 3, Name = "Sprite", Price = 7.4m });
            productList.Add(new Products { Id = 4, Name = "Crackers", Price = 2.9m });
            productList.Add(new Products { Id = 5, Name = "Pınar Su", Price = 1m });
            productList.Add(new Products { Id = 6, Name = "Fanta", Price = 3.7m });
            productList.Add(new Products { Id = 7, Name = "Popkek", Price = 0.8m });
            productList.Add(new Products { Id = 8, Name = "Çubuk Kraker", Price = 1.2m });
            productList.Add(new Products { Id = 9, Name = "Enerji İçeceği", Price = 6.4m });
            productList.Add(new Products { Id = 10, Name = "Kahve", Price = 2m });
            productList.Add(new Products { Id = 11, Name = "Soğuk Çay", Price = 4.8m });
            productList.Add(new Products { Id = 12, Name = "Cips", Price = 3.1m });

            return productList;
        }

        //id ile ürün getirme
        public static Products GetProduct(int id)
        {
            return ProductList().FirstOrDefault(x => x.Id == id);
        }

        //id'lere göre filtreleyip ürünleri listeleme
        public static List<Products> GetProducts(int[] ids)
        {
            return ProductList().Where(x => ids.Contains(x.Id)).ToList();
        }

        //Kampanyalar
        //mail'de verilen dataset'e göre dolduruldu
        public static List<Campaigns> CampaignList()
        {
            List<Campaigns> campaignList = new List<Campaigns>();

            campaignList.Add(new Campaigns
            {
                Id = 1,
                Products = new int[] { 1, 2 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 1, Price = 4m },
                    new ProductDto { Id = 2, Price = 6m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 2,
                Products = new int[] { 3, 7, 11 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 3, Price = 5m },
                    new ProductDto { Id = 7, Price = 0.5m },
                    new ProductDto { Id = 11, Price = 2m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 3,
                Products = new int[] { 9, 4 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 9, Price = 6m },
                    new ProductDto { Id = 4, Price = 2m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 4,
                Products = new int[] { 1, 4 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 1, Price = 3m },
                    new ProductDto { Id = 4, Price = 1.8m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 5,
                Products = new int[] { 1, 7 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 1, Price = 3.2m },
                    new ProductDto { Id = 7, Price = 0.7m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 6,
                Products = new int[] { 7, 11 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 7, Price = 0.4m },
                    new ProductDto { Id = 11, Price = 3.9m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 7,
                Products = new int[] { 3, 1 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 3, Price = 5.8m },
                    new ProductDto { Id = 1, Price = 2.9m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 8,
                Products = new int[] { 4, 9, 11 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 4, Price = 2.1m },
                    new ProductDto { Id = 9, Price = 5.4m },
                    new ProductDto { Id = 11, Price = 3.7m }
                }
            });

            campaignList.Add(new Campaigns
            {
                Id = 9,
                Products = new int[] { 7, 3 },
                Prices = new List<ProductDto> {
                    new ProductDto { Id = 7, Price = 0m },
                    new ProductDto { Id = 3, Price = 7m }
                }
            });

            return campaignList;
        }
    }
}
