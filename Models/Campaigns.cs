using System.Collections.Generic;

namespace OtomatUygulamasi.Models
{
    //mail'de verilen kampanya verisinin yapısı
    public class Campaigns
    {
        public int Id { get; set; }
        public int[] Products { get; set; }
        public List<ProductDto> Prices { get; set; }
    }
}
