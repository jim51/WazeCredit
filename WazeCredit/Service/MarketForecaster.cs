using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Model;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public class MarketForecaster : IMarketForecaster
    {
        public MarketResult GetMarketPrediction()
        {
            return new MarketResult
            {
                MarketCondition = MarketCondition.StableUp
            };
        }
    }
}
