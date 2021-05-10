using OtomatUygulamasi.Models;
using OtomatUygulamasi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OtomatUygulamasi.Managers
{
    public class Manager : IService
    {
        //ürünleri listeler
        public List<Products> ProductList()
        {
            return DataSets.DataSets.ProductList();
        }

        //kampanyaları listeler
        public List<Campaigns> CampaignList()
        {
            return DataSets.DataSets.CampaignList();
        }

        //kullanıcının seçtiği ürünlere ve girdiği para miktarına göre hesaplama yapar
        public ReceiptDto CalculateChosenProducts(List<SelectedProduct> selectedProducts, decimal givenMoney)
        {
            //kullanıcının seçtiği ürünlere göre kampanyalı ve kampanyasız ürünleri listeler
            List<ReceiptProductDto> receiptProductDtoList = CombinigLists(selectedProducts);

            //seçilen ürünlerin kampanyalı ve kampanyasız fiyatlarına göre hesaplama yapar
            decimal totalPrice = CalculateTotalPrice(receiptProductDtoList);


            //kullanıcıya dönen sonuç
            ReceiptDto receiptDto = new ReceiptDto
            {
                ReceiptProductDtoList = receiptProductDtoList,//ürünlerin listesi
                Remainder = givenMoney - totalPrice,//kalan para miktarı
                Message = totalPrice > givenMoney ? "Yeterli Bakiyeniz Yoktur" : "",//varsa mesaj
                Status = totalPrice > givenMoney ? false : true //durum kontrolü - arayüzde kullanmak için
            };

            return receiptDto;
        }

        //satış işlemini gerçekleştirir
        public PaymentReceipt Selling(List<SelectedProduct> selectedProducts, decimal givenMoney)
        {
            //kullanıcının seçtiği ürünlere göre kampanyalı ve kampanyasız ürünleri listeler
            List<ReceiptProductDto> receiptProductDtoList = CombinigLists(selectedProducts);

            //seçilen ürünlerin kampanyalı ve kampanyasız fiyatlarına göre hesaplama yapar
            decimal totalPrice = CalculateTotalPrice(receiptProductDtoList);

            //kalan tutar
            decimal remainder = givenMoney - totalPrice;

            //kullanıcıya dönecek sonuç
            PaymentReceipt paymentReceipt = new PaymentReceipt();

            //girilen para miktarı ürünlerin toplam fiyatından düşükse
            if(remainder < 0)
            {
                paymentReceipt.GivenMoney = givenMoney;//girilen para miktarı
                paymentReceipt.ReceiptProductDtoList = receiptProductDtoList;//seçilen ürünlerin listesi
                paymentReceipt.Remainder = remainder;
                paymentReceipt.Message = "Yetersiz Bakiye. Satış Gerçekleştirilemedi";//kullanıcıya verilen mesaj - arayüzde kullanılır
                paymentReceipt.Status = false;//kullanıcıya işlemin gerçekleşmediğini belirtmek için kullanılır
            }
            else
            {
                paymentReceipt.GivenMoney = givenMoney;//girilen para miktarı
                paymentReceipt.ReceiptProductDtoList = receiptProductDtoList;//seçilen ürünlerin listesi
                paymentReceipt.Remainder = remainder;//kalan para miktarı
                paymentReceipt.Message = "Afiyet Olsun.";//faturaya yazılan mesaj
                paymentReceipt.Status = true;//kullanıcıya işlemin gerçekleştiğini belirtmek için kullanılır

                //fatura yazdırılır - text dosyasına yazar
                WriteBill(paymentReceipt);
            }

            return paymentReceipt;
        }

        //seçilen ürünlere göre kampanyalı ürünleri listeler
        private List<ReceiptProductDto> CampaignProducts(List<SelectedProduct> selectedProducts)
        {
            List<ReceiptProductDto> receiptProductDtoList = new List<ReceiptProductDto>();

            //eşleşen kampanyaları listeler
            List<Campaigns> matchedCampaigns = MatchedCampaignList(selectedProducts);

            //kampanyalı ürünlerin id'lerini verir
            int[] campaignProductIds = CampaignProductIds(matchedCampaigns);

            //bir ürün birden fazla kampanya içinde bulunabileceği için aralarındaki en düşük fiyatlı kampanyayı bulmak için döngü kuruldu
            for(int i = 0; i < campaignProductIds.Length; i++)
            {
                //kampanyalı ürün id'si
                int productId = campaignProductIds[i];

                //ürünün bulunduğu kampanyalar
                List<Campaigns> productCampaigns = matchedCampaigns.Where(x => x.Products.Contains(productId)).ToList();

                //en düşük ücreti bulmak için hayali fiyat - karşılaştırma yapmak için
                decimal price = 9999999;

                //bir ürün birden fazla kampanyada bulunabileceği için döngü kuruldu
                foreach(var itemProductCampaigns in productCampaigns)
                {
                    //ürünün bulunduğu kampanyadaki fiyatı
                    decimal productPrice = itemProductCampaigns.Prices.FirstOrDefault(x => x.Id == productId).Price;

                    //en düşük fiyatı bulmak için karşılaştırma yapılır
                    if(productPrice < price) price = productPrice;
                }

                //en düşük fiyatlı kampanya bulunduktan sonra dönen sonuç
                ReceiptProductDto receiptProductDto = new ReceiptProductDto
                {
                    Product = DataSets.DataSets.GetProduct(productId),//ürünün orjinal bilgileri
                    CampaignsPrice = price,//ürünün kampanyalı fiyatı
                    Amount = selectedProducts.Where(x => x.ProductId == productId).FirstOrDefault().Amount//kullanıcıdan gelen ürün miktarı
                };

                //kampanyalı ürünler listeye eklenir
                receiptProductDtoList.Add(receiptProductDto);
            }

            return receiptProductDtoList;
        }

        //seçilen ürünlere göre kampanyasız ürünleri listeler
        private List<ReceiptProductDto> WithoutCampaignProducts(List<SelectedProduct> selectedProducts)
        {
            List<ReceiptProductDto> receiptProductDtoList = new List<ReceiptProductDto>();

            //kampanyaları listeler
            List<Campaigns> matchedCampaigns = MatchedCampaignList(selectedProducts);

            //kampanyalardaki ürünlerin id'lerini verir
            int[] campaignProductIds = CampaignProductIds(matchedCampaigns);

            //seçilen ürünlerde kampanya uygulanmayan ürünlerin id'lerini verir
            int[] outCampaignProducts = selectedProducts.Where(x => !campaignProductIds.Contains(x.ProductId)).Select(x => x.ProductId).ToArray();

            //kampanyasız ürünlerin orjinal bilgilerini listeler
            List<Products> outCampaignProductList = DataSets.DataSets.GetProducts(outCampaignProducts);

            foreach(var itemOutCampaignProductList in outCampaignProductList)
            {
                //seçilen kampanyasız ürünlerin bilgilerini döner
                ReceiptProductDto receiptProductDto = new ReceiptProductDto
                {
                    Product = itemOutCampaignProductList,
                    CampaignsPrice = null,
                    Amount = selectedProducts.Where(x => x.ProductId == itemOutCampaignProductList.Id).FirstOrDefault().Amount
                };

                //kampanyasız ürünler listeye eklenir
                receiptProductDtoList.Add(receiptProductDto);
            }

            return receiptProductDtoList;
        }

        //seçilen ürünler kampanyalı ve kampanyasız kriterlerine göre ayrıştırıldıktan sonra tek liste haline getirilir
        private List<ReceiptProductDto> CombinigLists(List<SelectedProduct> selectedProducts)
        {
            //kampanyalı ürün listesi
            List<ReceiptProductDto> campaignProducts = CampaignProducts(selectedProducts);

            //kampanyasız ürün listesi
            List<ReceiptProductDto> withoutCampaignProducts = WithoutCampaignProducts(selectedProducts);

            //birleştirilen liste
            List<ReceiptProductDto> receiptProductDtoList = new List<ReceiptProductDto>();
            receiptProductDtoList.AddRange(campaignProducts);
            receiptProductDtoList.AddRange(withoutCampaignProducts);

            return receiptProductDtoList;
        }

        //seçilen ürünlerin varsa eşleşen kampanyaları döner
        private List<Campaigns> MatchedCampaignList(List<SelectedProduct> selectedProducts)
        {
            //kampanyaları listeler
            List<Campaigns> campaignList = CampaignList();

            //eşleşen kampanyaları tutar
            List<Campaigns> matchedCampaigns = new List<Campaigns>();

            foreach(var itemCampaign in campaignList)
            {
                //seçilen ürünlerin kampanya eşleşmelerini sayar
                int matchedProductsCount = selectedProducts.Where(x => itemCampaign.Products.Contains(x.ProductId)).Count();

                //kampanyadaki ürünlerin sayısı
                int itemCampaignProductsCount = itemCampaign.Products.Count();

                //eşleşenlerin sayısı ile kampanyadaki ürünlerin sayısı birbirine eşitse kampanya dahil olur
                //örnek : kampanyadaki ürünler[1,2,3] , eşleşen ürünler[1,2] ise kampanya uygulanmaz eğer eşleşen ürünler[1,2,3] ise kampanya dahil olur
                if(matchedProductsCount == itemCampaignProductsCount)
                    matchedCampaigns.Add(itemCampaign);
            }

            return matchedCampaigns;
        }

        //kampanyadaki ürünlerin id'lerini döner
        private int[] CampaignProductIds(List<Campaigns> campaigns)
        {
            //eşleşen ürün id'lerini tutar
            List<int> matchedProducts = new List<int>();

            //kampanyaların sayısını tutar
            int campaignsCount = campaigns.Count();

            for(int i = 0; i < campaignsCount; i++)
            {
                //kampanyadaki ürünleri ekler
                matchedProducts.AddRange(campaigns[i].Products);

                for(int j = i + 1; j < campaignsCount; j++)
                {
                    //birden fazla kampanya varsa kesişen id'leri ekler
                    matchedProducts.AddRange(campaigns[i].Products.Intersect(campaigns[j].Products).ToArray());
                }
            }

            //dizide ürün id'si bireden fazla varsa ayıklanır ve döndürülür
            return matchedProducts.Distinct().ToArray();
        }

        //ürünlerin kampanyalı ya da kampanyasız fiyatları ile hesaplama yapar ve döner
        private decimal CalculateTotalPrice(List<ReceiptProductDto> receiptProductDtos)
        {
            decimal totalPrice = 0;

            foreach(var itemReceiptProductDtoList in receiptProductDtos)
            {
                totalPrice += itemReceiptProductDtoList.CampaignsPrice == null ?
                    itemReceiptProductDtoList.Product.Price * itemReceiptProductDtoList.Amount :
                    (decimal)itemReceiptProductDtoList.CampaignsPrice * itemReceiptProductDtoList.Amount;
            }

            return totalPrice;
        }

        //satış işlemi gerçekleştiğinde faturayı dosyaya yazar
        private void WriteBill(PaymentReceipt paymentReceipt)
        {
            string billFormat = "Hoş Geldiniz\n\nMiktar - Ürün - Fiyat - Kampanyalı Fiyat\n";

            //seçilen ürünleri ekler
            foreach(var item in paymentReceipt.ReceiptProductDtoList)
            {
                string campaignPrice = item.CampaignsPrice == null ? "" : item.CampaignsPrice.ToString();

                billFormat += string.Format("{0} X - {1} - {2} - {3}\n", item.Amount, item.Product.Name, item.Product.Price, campaignPrice);
            }

            billFormat += "\nToplam Fiyat: " + (paymentReceipt.GivenMoney - paymentReceipt.Remainder) + "\n";
            billFormat += "Ödenen Para Mikatrı: " + paymentReceipt.GivenMoney + "\n";
            billFormat += "Para Üstü: " + paymentReceipt.Remainder + "\n\n";
            billFormat += paymentReceipt.Message;

            //her satış için uygulamanın klasöründe txt dosyası oluşturur
            using(StreamWriter writetext = new StreamWriter("Receipt_" + DateTime.Now.ToString("dd_M_yyyy_hh_mm_ss") + ".txt"))
            {
                writetext.WriteLine(billFormat);
            }
        }
    }
}
