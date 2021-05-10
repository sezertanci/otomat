namespace OtomatUygulamasi.Models
{
    //kullanıcı her ürün seçtiğinde kampanyalı olup olmadığını döndüren yapı
    public class ReceiptProductDto
    {
        public decimal? CampaignsPrice { get; set; }//varsa kampanya fiyatı
        public Products Product { get; set; }//ürünün kendisi
        public int Amount { get; set; }//ürün miktarı
    }
}
