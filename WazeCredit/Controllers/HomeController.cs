using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WazeCredit.Model;
using WazeCredit.Models;
using WazeCredit.Models.ViewModels;
using WazeCredit.Service;
using WazeCredit.Utiltity.AppSettingsClasses;

namespace WazeCredit.Controllers
{
    public class HomeController : Controller
    {
        public HomeVM homeVM { get; set; }


        /// 設置interface參數
        /// 設置為唯讀以避免被修改
        private readonly IMarketForecaster _marketForecaster;
        private readonly StripeSettings _stripeOptions;
        private readonly TwilioSettings _twilioOptions;
        private readonly SendGridSettings _sendGridOptions;
        private readonly WazeForecastSettings _wazeForccastOptions;

        public HomeController(IMarketForecaster marketForecaster,
            IOptions<StripeSettings> stripeOptions,
            IOptions<TwilioSettings> twilioOptions,
            IOptions<SendGridSettings> sendGridOptions,
            IOptions<WazeForecastSettings> wazeForccastOptions
            )
        {
            homeVM = new HomeVM();
            _marketForecaster = marketForecaster;
            _stripeOptions = stripeOptions.Value;
            _twilioOptions = twilioOptions.Value;
            _sendGridOptions = sendGridOptions.Value;
            _wazeForccastOptions = wazeForccastOptions.Value;
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

        public IActionResult AllConfigSettings()
        {
            List<string> messages = new List<string>();
            messages.Add($"Waze config - Forecast Tracker: " + _wazeForccastOptions.ForecastTrackerEnabled);
            messages.Add($"Stripe Publishable Key: " + _stripeOptions.PublishableKey);
            messages.Add($"Stripe Secret Key: " + _stripeOptions.SecretKey);
            messages.Add($"Send Grid Key: " + _sendGridOptions.SendGridKey);
            messages.Add($"Twilio Phone: " + _twilioOptions.PhoneNumber);
            messages.Add($"Twilio SID: " + _twilioOptions.AccountSid);
            messages.Add($"Twilio Token: " + _twilioOptions.AuthToken);
            return View(messages);
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
