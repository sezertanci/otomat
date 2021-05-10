using System.Collections.Generic;

namespace OtomatUygulamasi.Models
{
    //fatura yapısı
    public class PaymentReceipt
    {
        public decimal GivenMoney { get; set; }//ödenen para
        public decimal Remainder { get; set; }//kalan para
        public List<ReceiptProductDto> ReceiptProductDtoList { get; set; }//seçilen ürünlerin miktarı ve kampanya fiyatları
        public string Message { get; set; }//kullanıcıya verilen mesaj
        public bool Status { get; set; }//durum kontrolü
    }
}
