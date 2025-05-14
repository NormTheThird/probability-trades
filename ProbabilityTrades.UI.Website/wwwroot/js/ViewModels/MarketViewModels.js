function MarketDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.DataSource = ko.observable(data.dataSource || "");
    self.Name = ko.observable(data.name || "");
    self.DisplayName = ko.observable(data.displayName || "");
    self.Description = ko.observable(data.description || "");
    self.IsActive = ko.observable(data.isActive);
    self.IsDeleted = ko.observable(data.isDeleted);
    self.LastChangedBy = ko.observable(data.lastChangedBy || "");
    self.DateLastChanged = ko.observable(data.dateLastChanged);

    self.LastChanged = ko.computed(function () {
        return self.LastChangedBy() + ' (' + BaseModel.FormattedDateString(self.DateLastChanged()) + ')';
    });
}

var MarketViewModel = function () {
    var self = this;

    self.Markets = ko.observableArray([]);

    self.GetMarkets = function () {
        BaseModel.ApiCallAsync("markets/", "GET", null)
            .then(response => {
                if (response.success) {
                    BaseModel.DataTable.Clear("markets-datatable");
                    self.Markets($.map(response.data, function (market) {
                        return new MarketDataModel(market);
                    }));
                    BaseModel.DataTable.Refresh("markets-datatable", 10, 0, "desc", true);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.ChangeMarketStatus = function (market) {
        market.IsActive(!market.IsActive());
        var data = ko.toJSON(market);
        BaseModel.ApiCallAsync("markets/", "PUT", data)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                    market.IsActive(!market.IsActive());
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
                market.IsActive(!market.IsActive());
            });
    }

    self.Load = function () {
        self.GetMarkets();
    };
}