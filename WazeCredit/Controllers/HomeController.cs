using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WazeCredit.Data;
using WazeCredit.Data.Repository.IRepository;
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

        [BindProperty]
        public CreditApplication CreditModel { get; set; }

        /// 設置interface參數
        /// 設置為唯讀以避免被修改
        private readonly IMarketForecaster _marketForecaster;
        private readonly ICreditValidator _creditValidator;
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        //private readonly StripeSettings _stripeOptions;
        //private readonly TwilioSettings _twilioOptions;
        //private readonly SendGridSettings _sendGridOptions;
        private readonly WazeForecastSettings _wazeForccastOptions;


        public HomeController(IMarketForecaster marketForecaster, IOptions<WazeForecastSettings> wazeForccastOptions,
                ICreditValidator creditValidator,
                ApplicationDbContext applicationDbContext,
                ILogger<HomeController> logger,
                IUnitOfWork unitOfWork
            )
        {
            homeVM = new HomeVM();
            _logger = logger;
            _marketForecaster = marketForecaster;
            _wazeForccastOptions = wazeForccastOptions.Value;
            _creditValidator = creditValidator;
            _db = applicationDbContext;
            _unitOfWork = unitOfWork;
           
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home controller index start");

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
            _logger.LogInformation("Home controller index End");
            return View(homeVM);
        }

        public IActionResult AllConfigSettings(
            [FromServices] IOptions<StripeSettings> striptOptions,
            [FromServices] IOptions<TwilioSettings> twilioOptions,
            [FromServices] IOptions<SendGridSettings> sendGridOptions

            )
        {
            List<string> messages = new List<string>();
            messages.Add($"Waze config - Forecast Tracker: " + _wazeForccastOptions.ForecastTrackerEnabled);
            messages.Add($"Stripe Publishable Key: " + striptOptions.Value.PublishableKey);
            messages.Add($"Stripe Secret Key: " + striptOptions.Value.SecretKey);
            messages.Add($"Send Grid Key: " + sendGridOptions.Value.SendGridKey);
            messages.Add($"Twilio Phone: " + twilioOptions.Value.PhoneNumber);
            messages.Add($"Twilio SID: " + twilioOptions.Value.AccountSid);
            messages.Add($"Twilio Token: " + twilioOptions.Value.AuthToken);
            return View(messages);
        }

        public IActionResult CreditApplication()
        {
            CreditModel = new CreditApplication();
            return View(CreditModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ActionName("CreditApplication")]
        public async Task<IActionResult> CreditApplicationPOST(
            [FromServices] Func<CreditApprovedEnum,ICreditApproved> _creditService
            )
        {
            if (ModelState.IsValid)
            {
                var (validationPassed, errorMessages) = await _creditValidator.PassAllValidations(CreditModel);

                CreditResult creditResult = new CreditResult()
                {
                    ErrorList = errorMessages,
                    Success = validationPassed,
                    CreditID = 0
                };
                if (validationPassed)
                {
                    CreditModel.CreditApproved = 
                        _creditService(CreditModel.Salary > 50000 ? CreditApprovedEnum.High : CreditApprovedEnum.Low)
                        .GetCreditApproved(CreditModel);
                    _unitOfWork.CreditApplication.Add(CreditModel);
                    _unitOfWork.Save();
                    creditResult.CreditID = CreditModel.Id;
                    creditResult.CreditApproved = CreditModel.CreditApproved;
                    return RedirectToAction(nameof(CreditResult),creditResult);
                }
                else
                {
                    return RedirectToAction(nameof(CreditResult), creditResult);
                }
            }
            return View(CreditModel);
        }


        public IActionResult CreditResult(CreditResult creditResult)
        {
            return View(creditResult);
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
