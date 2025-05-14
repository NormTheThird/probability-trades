var MarketActionsViewModel = function () {
    var self = this;

    self.IsShowSelection = ko.observable(true);
    self.IsShowSelection.subscribe(function (value) {
        if (value) {
            self.IsShowMovingAverages(false);
        }
    });
    self.IsShowMovingAverages = ko.observable(false);
    self.IsShowMovingAverages.subscribe(function (value) {
        if (value) {
            self.IsShowSelection(false);
        }
    });
    self.MovingAverageVM = ko.observable(new MovingAverageViewModel());

    self.ShowMovingAverages = function () {
        self.IsShowMovingAverages(true);
        self.MovingAverageVM().Load();
    }

    self.ShowSelection = function () {
        self.IsShowSelection(true);
    }

    self.Load = function () {

    };
}


// Moving Averages

function MovingAverageConfigurationSummaryDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.Active = ko.observable(false);

    self.DisplayName = ko.computed(function () {
        return self.BaseCurrency() + '-' + self.QuoteCurrency();
    });
}

function MovingAverageConfigurationDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.DataSource = ko.observable(data.dataSource || "Kucoin");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.CandlestickPattern = ko.observable(data.candlestickPattern || "");
    self.ShortMovingAverageDays = ko.observable(data.shortMovingAverageDays || 0);
    self.LongMovingAverageDays = ko.observable(data.longMovingAverageDays || 0);
    self.StopLossPercentage = ko.observable(data.stopLossPercentage || 0.00);
    self.IsActive = ko.observable(data.isActive);
    self.IsSendDiscordNotification = ko.observable(data.isSendDiscordNotification);

    self.StatusDisplay = ko.computed(function () {
        return self.IsActive() ? 'Active' : 'Disabled';
    });
    self.NotificationDisplay = ko.computed(function () {
        return self.IsSendDiscordNotification() ? 'On' : 'Off';;
    });
}

function MovingAverageStatusSummaryDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.CandlestickPattern = ko.observable(data.candlestickPattern || "");
    self.Active = ko.observable(false);
}

function MovingAverageStatusDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.DataSource = ko.observable(data.dataSource || "");
    self.BaseCurrency = ko.observable(data.baseCurrency || "");
    self.QuoteCurrency = ko.observable(data.quoteCurrency || "");
    self.CandlestickPattern = ko.observable(data.candlestickPattern || "");
    self.CloseDate = ko.observable(data.closeDate);
    self.ClosePrice = ko.observable(data.closePrice.toFixed(2) || 0.00);
    self.MarketAction = ko.observable(data.marketAction);
    self.IsActionChange = ko.observable(data.isActionChange);
    self.ShortMovingAverageDays = ko.observable(data.shortMovingAverageDays || 0);
    self.ShortMovingAverage = ko.observable(data.shortMovingAverage.toFixed(2) || 0.00);
    self.LongMovingAverageDays = ko.observable(data.longMovingAverageDays || 0);
    self.LongMovingAverage = ko.observable(data.longMovingAverage.toFixed(2) || 0.00);

    self.DaysDisplay = ko.computed(function () {
        return self.ShortMovingAverageDays() + ' / ' + self.LongMovingAverageDays();
    });
    self.CloseDateDisplay = ko.computed(function () {
        return BaseModel.FormattedDateString(self.CloseDate());
    });
}

var MovingAverageViewModel = function () {
    var self = this;

    self.ConfigurationSummary = ko.observableArray([]);
    self.SelectedConfigurationSummary = ko.observable(new MovingAverageConfigurationSummaryDataModel());
    self.SelectedConfigurationSummary.subscribe(function (summary) {
        if (summary) {
            self.FilterConfigurations(summary);
            self.GetStatuses(summary);
        }
    });
    self.AllConfigurations = ko.observableArray([]);
    self.Configurations = ko.observableArray([]);
    self.SelectedConfiguration = ko.observable(new MovingAverageConfigurationDataModel());
    self.StatusSummary = ko.observableArray([]);
    self.SelectedStatusSummary = ko.observable(new MovingAverageStatusSummaryDataModel());
    self.SelectedStatusSummary.subscribe(function (summary) {
        if (summary) {
            self.FilterStatuses(summary);
        }
    });
    self.AllStatuses = ko.observableArray([]);
    self.Statuses = ko.observableArray([]);

    self.GetConfigurations = function () {
        BaseModel.ApiCallAsync("movingaverage/kucoin", "GET", null)
            .then(response => {
                if (response.success) {
                    self.ConfigurationSummary([]);
                    self.AllConfigurations($.map(response.data, function (configuration) {
                        self.AddToConfigurationSummary(configuration);
                        return new MovingAverageConfigurationDataModel(configuration);
                    }));

                    self.SelectConfigurationSummary(self.ConfigurationSummary()[0]);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.AddToConfigurationSummary = function (configuration) {
        var match = ko.utils.arrayFirst(self.ConfigurationSummary(), function (summary) {
            return summary.DataSource() === configuration.dataSource &&
                summary.BaseCurrency() === configuration.baseCurrency &&
                summary.QuoteCurrency() === configuration.quoteCurrency;
        });
        if (match === undefined) {
            var movingAverageConfigurationSummaryDataModel = new MovingAverageConfigurationSummaryDataModel();
            movingAverageConfigurationSummaryDataModel.DataSource(configuration.dataSource);
            movingAverageConfigurationSummaryDataModel.BaseCurrency(configuration.baseCurrency);
            movingAverageConfigurationSummaryDataModel.QuoteCurrency(configuration.quoteCurrency);
            self.ConfigurationSummary.push(movingAverageConfigurationSummaryDataModel);
        }
    };

    self.SelectConfigurationSummary = function (selectedConfigurationSummary) {
        ko.utils.arrayFirst(self.ConfigurationSummary(), function (configurationSummary) {
            configurationSummary.Active(false);
        });
        self.SelectedConfigurationSummary(selectedConfigurationSummary);
        self.SelectedConfigurationSummary().Active(true);
    };

    self.FilterConfigurations = function (configurationSummary) {
        self.Configurations([]);
        ko.utils.arrayFirst(self.AllConfigurations(), function (configuration) {
            if (configuration.DataSource() === configurationSummary.DataSource() &&
                configuration.BaseCurrency() === configurationSummary.BaseCurrency() &&
                configuration.QuoteCurrency() === configurationSummary.QuoteCurrency()) {
                self.Configurations.push(configuration);
            }
        });
    };

    self.GetStatuses = function (configurationSummary) {
        var symbol = `${configurationSummary.BaseCurrency()}-${configurationSummary.QuoteCurrency()}`;
        BaseModel.ApiCallAsync(`movingaverage/${configurationSummary.DataSource()}/${symbol}/0`, "GET", null)
            .then(response => {
                if (response.success) {
                    self.StatusSummary([]);
                    self.Statuses([]);
                    self.AllStatuses($.map(response.data, function (status) {
                        self.AddToStatusSummary(status);
                        return new MovingAverageStatusDataModel(status);
                    }));

                    self.SelectStatusSummary(self.StatusSummary()[0]);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.AddToStatusSummary = function (status) {
        var match = ko.utils.arrayFirst(self.StatusSummary(), function (summary) {
            return summary.CandlestickPattern() === status.candlestickPattern;
        });
        if (match === undefined) {
            var movingAverageStatusSummaryDataModel = new MovingAverageStatusSummaryDataModel();
            movingAverageStatusSummaryDataModel.CandlestickPattern(status.candlestickPattern);
            self.StatusSummary.push(movingAverageStatusSummaryDataModel);
        }
    };

    self.SelectStatusSummary = function (selectedStatusSummary) {
        ko.utils.arrayFirst(self.StatusSummary(), function (statusSummary) {
            statusSummary.Active(false);
        });

        if (selectedStatusSummary !== undefined) {
            self.SelectedStatusSummary(selectedStatusSummary);
            self.SelectedStatusSummary().Active(true);
        }
    };

    self.FilterStatuses = function (statusSummary) {
        self.Statuses([]);
        BaseModel.DataTable.Clear("moving-average-status-datatable");
        ko.utils.arrayFirst(self.AllStatuses(), function (status) {
            if (status.CandlestickPattern() === statusSummary.CandlestickPattern()) {
                self.Statuses.push(status);
            }
        });
        BaseModel.DataTable.Refresh("moving-average-status-datatable", 10, 0, "desc", false);
    };

    self.ShowAddConfiguration = function () {
        self.SelectedConfiguration(new MovingAverageConfigurationDataModel());
        self.SelectedConfiguration().IsActive(true);
        $(".moving-average-configuration-modal").modal("show");
    };

    self.ShowEditConfiguration = function (configuration) {
        self.SelectedConfiguration(configuration);
        $(".moving-average-configuration-modal").modal("show");
    };

    self.AddConfiguration = function () {
        var data = ko.toJSON(self.SelectedConfiguration());
        BaseModel.ApiCallAsync("movingaverage", "POST", data)
            .then(response => {
                if (response.success) {
                    self.GetConfigurations();
                    $(".moving-average-configuration-modal").modal("hide");
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.SaveConfiguration = function () {
        var data = ko.toJSON(self.SelectedConfiguration());
        BaseModel.ApiCallAsync(`movingaverage/${self.SelectedConfiguration().Id()}`, "PUT", data)
            .then(response => {
                if (response.success) {
                    $(".moving-average-configuration-modal").modal("hide");
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.DeleteConfiguration = function (configuration) {
        var retval = confirm("Are you sure you want to delete this configuration?");
        if (retval !== true) {
            return;
        }

        BaseModel.ApiCallAsync(`movingaverage/${configuration.Id()}`, "DELETE", null)
            .then(response => {
                if (response.success) {
                    self.Configurations.remove(configuration);
                    self.AllConfigurations.remove(configuration);
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
        self.GetConfigurations();
    };
}