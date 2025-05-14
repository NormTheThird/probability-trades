using QuantConnect;
using QuantConnect.Algorithm.Framework;
using QuantConnect.Algorithm.Framework.Selection;
using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Algorithm.Framework.Risk;
using QuantConnect.Algorithm.Selection;
using QuantConnect.Parameters;
using QuantConnect.Benchmarks;
using QuantConnect.Brokerages;
using QuantConnect.Util;
using QuantConnect.Interfaces;
using QuantConnect.Algorithm;
using QuantConnect.Indicators;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Custom;
using QuantConnect.DataSource;
using QuantConnect.Data.Fundamental;
using QuantConnect.Data.Market;
using QuantConnect.Data.UniverseSelection;
using QuantConnect.Notifications;
using QuantConnect.Orders;
using QuantConnect.Orders.Fees;
using QuantConnect.Orders.Fills;
using QuantConnect.Orders.Slippage;
using QuantConnect.Scheduling;
using QuantConnect.Securities;
using QuantConnect.Securities.Equity;
using QuantConnect.Securities.Future;
using QuantConnect.Securities.Option;
using QuantConnect.Securities.Forex;
using QuantConnect.Securities.Crypto;
using QuantConnect.Securities.Interfaces;
using QuantConnect.Storage;
using QCAlgorithmFramework = QuantConnect.Algorithm.QCAlgorithm;
using QCAlgorithmFrameworkBridge = QuantConnect.Algorithm.QCAlgorithm;
using System.Security.Cryptography;

namespace ProbabilityTrades.Console.Tests;

internal class QuantConnectTest : QCAlgorithm
{
    private Equity SPYEquity;
    private Equity QQQEquity;
    private Equity TLTEquity;

    private SimpleMovingAverage NewMA;


    public override void Initialize()
    {
        SetStartDate(2020, 1, 4);
        SetEndDate(2023, 6, 6);
        SetCash(100000);

        //AddEquity("SPY", Resolution.Daily);
        SPYEquity = AddEquity("SPY", Resolution.Daily);
        //QQQEquity = AddEquity("QQQ", Resolution.Daily);
        //TLTEquity = AddEquity("TLT", Resolution.Daily);
        NewMA = SMA(SPYEquity.Symbol, 100);

        SetWarmUp(100);
    }

    /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
    /// Slice object keyed by symbol containing the stock data
    public override void OnData(Slice data)
    {
        if (IsWarmingUp)
            return;

        if (!Portfolio.Invested)
        {
            if (SPYEquity.Price > NewMA.Current.Value)
            {
                MarketOrder(SPYEquity.Symbol, 1);
                Debug("Current Price is > " + SPYEquity.Price + " MA: " + NewMA.Current.Value);
            }
        }

        if (Portfolio.Invested)
        {
            if (SPYEquity.Price < NewMA.Current.Value)
            {
                Liquidate();
                Debug("Current Price is < " + SPYEquity.Price + " MA: " + NewMA.Current.Value);
            }
        }

        //if (!Portfolio.Invested)
        //{
        //SetHoldings(SPYEquity.Symbol, .033);
        //SetHoldings(QQQEquity.Symbol, .033);
        //SetHoldings(TLTEquity.Symbol, .033);
        //}

        //Debug("Number of SPY shares " + Portfolio[SPYEquity.Symbol].Quantity + " @Price " + Portfolio[SPYEquity.Symbol].AveragePrice);
        //Debug("Number of QQQ shares " + Portfolio[QQQEquity.Symbol].Quantity + " @Price " + Portfolio[QQQEquity.Symbol].AveragePrice);
        //Debug("Number of TLT shares " + Portfolio[TLTEquity.Symbol].Quantity + " @Price " + Portfolio[TLTEquity.Symbol].AveragePrice);

        Plot(SPYEquity.Symbol, "MA30", NewMA.Current.Value);
        Plot(SPYEquity.Symbol, SPYEquity.Symbol, SPYEquity.Price);
    }
}