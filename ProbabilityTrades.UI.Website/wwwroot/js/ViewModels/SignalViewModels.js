function SignalStatusDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.CloseDate = ko.observable(data.closeDate);
    self.ClosePrice = ko.observable(data.closePrice.toFixed(2) || 0.00);
    self.MarketAction = ko.observable(data.marketAction);

    self.CloseDateDisplay = ko.computed(function () {
        return BaseModel.FormattedDateString(self.CloseDate());
    })

    self.MarketActionClass = ko.computed(function () {
        return self.MarketAction().includes("Enter") ? "text-success" : "text-danger";
    });
}

var SignalsViewModel = function () {
    var self = this;

    self.Signals = ko.observableArray([]);

    self.GetSignals = function () {
        BaseModel.ApiCallAsync("movingaverage/kucoin/BTC-USDT/actions/0", "GET", null)
            .then(response => {
                if (response.success) {
                    BaseModel.DataTable.Clear("moving-average-singnal-datatable");
                    self.Signals($.map(response.data, function (status) {
                        return new SignalStatusDataModel(status);
                    }));
                    BaseModel.DataTable.Refresh("moving-average-singnal-datatable", 10, 0, "desc", false);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.Load = function () {
        self.GetSignals();
    };
}