using Microsoft.AspNetCore.Mvc;
using OtomatUygulamasi.Models;
using OtomatUygulamasi.Services;
using System.Collections.Generic;

namespace OtomatUygulamasi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService _service;

        public HomeController(IService service)
        {
            _service = service;
        }

        //açılış ekranı
        public ActionResult Index()
        {
            return View();
        }

        //ürünleri listeler
        public IActionResult Products()
        {
            return Json(DataSets.DataSets.ProductList());
        }

        //her ürün seçiminde hesaplama yaptırır
        [HttpPost]
        public IActionResult Calculate([FromBody] List<SelectedProduct> products, decimal givenMoney)
        {
            return Json(_service.CalculateChosenProducts(products, givenMoney));
        }

        //satış işlemini gerçekleştirir
        [HttpPost]
        public IActionResult Selling([FromBody] List<SelectedProduct> products, decimal givenMoney)
        {
            return Json(_service.Selling(products, givenMoney));
        }
    }
}
