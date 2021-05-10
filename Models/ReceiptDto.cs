using System.Collections.Generic;

namespace OtomatUygulamasi.Models
{
    //kullanıcı her ürün seçtiğinde hesaplanıp dönen yapı
    public class ReceiptDto
    {
        public decimal Remainder { get; set; }//kalan para
        public List<ReceiptProductDto> ReceiptProductDtoList { get; set; }//seçilen ürünlerin miktarı ve kampanya fiyatları

        public string Message { get; set; }//kullanıcıyı uyaran mesaj
        public bool Status { get; set; }//durum kontrolü - arayüzde kullanmak için
    }
}
