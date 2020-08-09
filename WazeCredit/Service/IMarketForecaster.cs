using WazeCredit.Model;

namespace WazeCredit.Service
{
    public interface IMarketForecaster
    {
        MarketResult GetMarketPrediction();
    }
}