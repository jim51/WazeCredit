using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WazeCredit.Model;
using WazeCredit.Models;
using WazeCredit.Models.ViewModels;
using WazeCredit.Service;

namespace WazeCredit.Controllers
{
    public class HomeController : Controller
    {
        public HomeVM homeVM { get; set; }
        /// 設置interface參數
        /// 設置為唯讀以避免被修改
        private readonly IMarketForecaster _marketForecaster;
        public HomeController(IMarketForecaster marketForecaster)
        {
            homeVM = new HomeVM();
            _marketForecaster = marketForecaster;
        }

        public IActionResult Index()
        {
            
            // 宣告類別
            // 刪除直接宣告
            //MarketForecasterV2 marketForecaster = new MarketForecasterV2();

            // 取得資料
            // 取得資料改由
            MarketResult currentMarket = _marketForecaster.GetMarketPrediction();

            switch (currentMarket.MarketCondition)
            {
                case MarketCondition.StableUp:
                    homeVM.MarketForecast = "市場穩定上升";
                    break;
                case MarketCondition.StableDown:
                    homeVM.MarketForecast = "市場穩定下降";
                    break;
                case MarketCondition.Volatile:
                    homeVM.MarketForecast = "市場波動";
                    break;
                default:
                    homeVM.MarketForecast = "Apply for a credit card using our application!";
                    break;
            }
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
