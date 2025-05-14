function AccountDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.FirstName = ko.observable(data.firstName || "");
    self.LastName = ko.observable(data.lastName || "");
    self.Username = ko.observable(data.username || "");
    self.Email = ko.observable(data.email || "");
    self.PhoneNumber = ko.observable(data.phoneNumber || "");
    self.IsAdmin = ko.observable(false);
}

function ChangePasswordDataModel() {
    var self = this;

    self.UserId = ko.observable(BaseModel.Guid.Empty);
    self.NewPassword = ko.observable("");
    self.ConfirmPassword = ko.observable("");
    self.CurrentPassword = ko.observable("");
}

function UserExchangeSummaryDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Name = ko.observable(data.name || "");
    self.Active = ko.observable(false);
}

function UserExchangeDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.UserId = ko.observable(data.userId || BaseModel.Guid.Empty);
    self.Name = ko.observable(data.name || "");
    self.APIKey = ko.observable(data.apiKey || "");
    self.APISecret = ko.observable(data.apiSecret || "");
    self.APIPassphrase = ko.observable(data.apiPassphrase || "");
}

var AccountViewModel = function () {
    var self = this;

    self.UserId = ko.observable(Cookies.get("userId"));
    self.Account = ko.observable(new AccountDataModel());
    self.ChangePassword = ko.observable(new ChangePasswordDataModel());
    self.UserExchangeSummary = ko.observableArray([]);
    self.SelectedUserExchangeSummary = ko.observable(new UserExchangeSummaryDataModel());
    self.SelectedUserExchangeSummary.subscribe(function (userExchangeSummary) {
        self.SelectedUserExchange(new UserExchangeDataModel());
        if (userExchangeSummary) {
            ko.utils.arrayFirst(self.UserExchanges(), function (userExchange) {
                if (userExchange.Name() === userExchangeSummary.Name()) {
                    self.SelectedUserExchange(userExchange);
                }
            });
        }
    });
    self.UserExchanges = ko.observableArray([]);
    self.SelectedUserExchange = ko.observable(new UserExchangeDataModel());

    self.GetAccount = function () {
        BaseModel.ApiCallAsync(`users/${self.UserId()}`, "GET", null)
            .then(response => {
                if (response.success) {
                    self.Account(new AccountDataModel(response.data));
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            }).catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.GetUserExchanges = function () {
        BaseModel.ApiCallAsync(`users/${self.UserId()}/user-exchanges`, "GET", null)
            .then(response => {
                if (response.success) {
                    self.UserExchanges($.map(response.data, function (userExchange) {
                        self.AddToUserExchangeSummary(userExchange);
                        return new UserExchangeDataModel(userExchange);
                    }));

                    if (self.UserExchangeSummary().length > 0) {
                        self.SelectUserExchangeSummary(self.UserExchangeSummary()[0]);
                    }
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            }).catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.AddToUserExchangeSummary = function (userExchange) {
        var match = ko.utils.arrayFirst(self.UserExchangeSummary(), function (summary) {
            return summary.Name() === userExchange.name;
        });
        if (match === undefined) {
            var userExchangeSummaryDataModel = new UserExchangeSummaryDataModel();
            userExchangeSummaryDataModel.Name(userExchange.name);
            self.UserExchangeSummary.push(userExchangeSummaryDataModel);
        }
    };

    self.SelectUserExchangeSummary = function (selectedUserExchangeSummary) {
        ko.utils.arrayFirst(self.UserExchangeSummary(), function (userExchangeSummary) {
            userExchangeSummary.Active(false);
        });
        self.SelectedUserExchangeSummary(selectedUserExchangeSummary);
        self.SelectedUserExchangeSummary().Active(true);
    };

    self.SaveAccount = function () {
        var data = ko.toJSON(self.Account());
        BaseModel.ApiCallAsync("users/", "PUT", data)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.SaveAPIKeys = function () {
        console.log("Save API Keys");
    };

    self.ChangeAccountPassword = function () {
        self.ChangePassword().UserId(self.UserId());
        var data = ko.toJSON(self.ChangePassword());
        BaseModel.ApiCallAsync("security/change-password/", "PUT", data)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success);
                    self.ChangePassword(new ChangePasswordDataModel());
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage);
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    }

    self.Load = function () {
        this.GetAccount();
        this.GetUserExchanges();
    };
}