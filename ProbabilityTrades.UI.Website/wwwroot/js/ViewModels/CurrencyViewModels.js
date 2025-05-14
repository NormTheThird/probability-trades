function CurrencyHistorySummaryDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");

    self.DisplayName = ko.computed(function () {
        return self.BaseCurrency() + '-' + self.QuoteCurrency();
    });
};

function CurrencyHistorySummaryDetailDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.CandlePattern = ko.observable(data.candlePattern || "");
    self.MinTimeEpoch = ko.observable(data.minTimeEpoch || 0);
    self.MinDate = ko.observable(data.minDate);
    self.MaxTimeEpoch = ko.observable(data.maxTimeEpoch || 0);
    self.MaxDate = ko.observable(data.maxDate);
    self.LastUpdatedAt = ko.observable(data.lastUpdatedAt);
    self.NumberOfRecords = ko.observable(data.numberOfRecords || 0);
    self.MaxRecords = ko.observable(data.maxRecords || 0);
    self.Active = ko.observable(false);
};

function CurrencyHistoryDetailsDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.CandlePattern = ko.observable(data.candlePattern || "");
    self.ChartTimeEpoch = ko.observable(data.chartTimeEpoch || 0);
    self.ChartTimeIso = ko.observable(data.chartTimeIso);
    self.OpenPrice = ko.observable(data.openPrice || 0.00);
    self.ClosePrice = ko.observable(data.closePrice|| 0.00);
    self.HighPrice = ko.observable(data.highPrice || 0.00);
    self.LowPrice = ko.observable(data.lowPrice || 0.00);
    self.Volume = ko.observable(data.volume.toFixed(3) || 0.00);
    self.Turnover = ko.observable(data.turnover.toFixed(3) || 0.00);

    self.DateDisplay = ko.computed(function () {
        return BaseModel.FormattedDateString(self.ChartTimeIso());
    })
};

function ActiveMarketDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.MarketId = ko.observable(data.id || BaseModel.Guid.Empty);
    self.DataSource = ko.observable(data.dataSource || "");
    self.Name = ko.observable(data.name || "");
    self.DisplayName = ko.observable(data.displayName || "");
};

function AddCurrencyDataModel() {
    var self = this;

    self.CandlePattern = ko.observable("1day");
    self.MarketId = ko.observable();
    self.Symbol = ko.observable();
    self.StartDate = ko.observable(new Date());
    self.EndDate = ko.observable(new Date());
};

var CurrencyViewModel = function () {
    var self = this;

    self.CurrencyHistorySummary = ko.observableArray([]);
    self.CurrencyHistorySummaryDetail = ko.observableArray([]);
    self.CurrencyHistoryDetails = ko.observableArray([]);
    self.NewCurrency = ko.observable(new AddCurrencyDataModel());
    self.SelectedCurrencyHistorySummary = ko.observable(new CurrencyHistorySummaryDataModel());
    self.SelectedCurrencyHistorySummaryDetail = ko.observable(new CurrencyHistorySummaryDetailDataModel());
    self.SelectedCurrencyHistorySummaryDetail.subscribe(function (detail) {
        if (detail) {
            self.GetCurrencyHistoryDetails(detail);
        }
    });
    self.Markets = ko.observableArray([]);
    self.Symbols = ko.observableArray([]);
    self.IsShowSummary = ko.observable(false);
    self.IsShowSummary.subscribe(function (value) {
        if (value) {
            self.IsShowDetail(false);
        }
    });
    self.IsShowDetail = ko.observable(false);
    self.IsShowDetail.subscribe(function (value) {
        if (value) {
            self.IsShowSummary(false);
        }
    });

    self.GetCurrencyHistorySummary = function () {
        BaseModel.ApiCallAsync("currencyhistory/", "GET", null)
            .then(response => {
                if (response.success) {
                    BaseModel.DataTable.Clear("currency-history-datatable");
                    self.CurrencyHistorySummary($.map(response.data, function (currency) {
                        return new CurrencyHistorySummaryDataModel(currency);
                    }));
                    BaseModel.DataTable.Refresh("currency-history-datatable", 10, 0, "desc", true);

                    self.IsShowSummary(true);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.GetCurrencyHistorySummaryDetail = function (summary) {
        self.SelectedCurrencyHistorySummary(summary);
        BaseModel.ApiCallAsync(`currencyhistory/${summary.DataSource()}/${summary.BaseCurrency()}-${summary.QuoteCurrency()}`, "GET", null)
            .then(response => {
                if (response.success) {
                    self.CurrencyHistorySummaryDetail($.map(response.data, function (detail) {
                        return new CurrencyHistorySummaryDetailDataModel(detail);
                    }));
                    self.SelectedCurrencyHistorySummaryDetail(self.CurrencyHistorySummaryDetail()[0]);
                    self.SelectedCurrencyHistorySummaryDetail().Active(true);
                    self.IsShowDetail(true);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.GetCurrencyHistoryDetails = function (summaryDetail) {
        self.CurrencyHistoryDetails([]);
        BaseModel.ApiCallAsync(`currencyhistory/${summaryDetail.DataSource()}/${summaryDetail.BaseCurrency()}-${summaryDetail.QuoteCurrency()}/${summaryDetail.CandlePattern()}/0/100`, "GET", null)
            .then(response => {
                if (response.success) {               
                    self.CurrencyHistoryDetails($.map(response.data, function (detail) {
                        return new CurrencyHistoryDetailsDataModel(detail);
                    }));
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.DownloadCurrencyHistoryDetails = function () {
        console.log(self.SelectedCurrencyHistorySummary());
        BaseModel.Message(BaseModel.MessageLevels.Error, "Not yet implemented JASON! Stop clicking this button.");
    };

    self.BackToSummary = function () {
        self.IsShowSummary(true);
    };

    self.GetActiveMarkets = function () {
        BaseModel.ApiCallAsync("markets/active", "GET", null)
            .then(response => {
                if (response.success) {
                    self.Markets($.map(response.data, function (market) {
                        return new ActiveMarketDataModel(market);
                    }));
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.SetStartDateBackFourYears = function () {
        var fourYearsAgo = new Date();
        fourYearsAgo.setFullYear(fourYearsAgo.getFullYear() - 4);
        self.NewCurrency().StartDate(fourYearsAgo);
    }

    self.AddCurrency = function () {
        var data = ko.toJSON(self.NewCurrency());
        console.log(data);
    };

    self.ShowSummary = function () { }

    self.Load = function () {
        self.GetCurrencyHistorySummary();
        self.GetActiveMarkets();
    };
}