using OtomatUygulamasi.Models;
using System.Collections.Generic;

namespace OtomatUygulamasi.Services
{
    //controller içinde çağrılan servis
    public interface IService
    {
        List<Products> ProductList();//ürünleri listeler
        ReceiptDto CalculateChosenProducts(List<SelectedProduct> selectedProducts, decimal givenMoney);//seçilen ürünlere ve girilen para miktarına göre hesaplama yapar
        PaymentReceipt Selling(List<SelectedProduct> selectedProducts, decimal givenMoney);//satış işlemini gerçekleştirir
    }
}
