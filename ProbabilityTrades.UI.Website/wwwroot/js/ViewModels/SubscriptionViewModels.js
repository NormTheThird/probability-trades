function AvailableSubscriptionDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.Title = ko.observable(data.title || "");
    self.CurrentPrice = ko.observable(data.currentPrice || 0.0);
};

function UserSubscriptionDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.SubscriptionId = ko.observable(data.subscriptionId || BaseModel.Guid.Empty);
    self.SubscriptionDetailId = ko.observable(data.subscriptionDetailId || BaseModel.Guid.Empty);
    self.SubscriptionType = ko.observable(data.subscriptionType || "");
    self.AmountPaid = ko.observable(data.amountPaid || 0.0);
    self.StartDate = ko.observable(BaseModel.FormattedDateString(data.startDate));
    self.EndDate = ko.observable(BaseModel.FormattedDateString(data.endDate));
    self.IsCurrent = ko.observable(data.isCurrent || false);

    self.AmountPaidDisplay = ko.computed(function () {
        return '$' + self.AmountPaid().toFixed(2);
    });
    self.StatusDisplay = ko.computed(function () {
        return self.IsCurrent()
            ? '<span class="text-success">Active</span>'
            : '<span class="text-warning">Expired</span>';
    });
}

function PaymentMethodDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.UserId = ko.observable(data.userId || BaseModel.Guid.Empty);
}

var SubscriptionViewModel = function () {
    var self = this;

    self.AvailableSubscriptions = ko.observableArray([]);
    self.UserSubscriptions = ko.observableArray([]);
    self.MonthlySubscription = ko.observable(new AvailableSubscriptionDataModel());
    self.YearlySubscription = ko.observable(new AvailableSubscriptionDataModel());
    self.SelectedSubscription = ko.observable(new AvailableSubscriptionDataModel());
    self.SelectedSubscription.subscribe(function (subscription) {
        if (subscription === undefined || subscription === null) {
            return;
        }

        self.SubTotal(subscription.CurrentPrice());
        if (self.AppliedDiscountCode() !== undefined && self.AppliedDiscountCode() !== null && self.AppliedDiscountCode() !== "") {
            self.ValidateDiscountCodeWithCode(self.AppliedDiscountCode());
        }
    })
    self.SelectedSubscriptionId = ko.observable(BaseModel.Guid.Empty);
    self.SubTotal = ko.observable(0.0);
    self.DiscountCode = ko.observable();
    self.AppliedDiscountCode = ko.observable();
    self.AppliedDiscountCodeId = ko.observable();
    self.Discount = ko.observable("");
    self.DiscountAmount = ko.observable(0.0);
    self.IsShowDiscountCodeError = ko.observable(false);
    self.ShowSubscribeForm = ko.observable(false);
    self.ShowUserSubscriptions = ko.observable(false);
    self.CurrentlySubscribed = ko.observable(false);
    self.HasCardOnFile = ko.observable(false);

    self.DiscountDescription = ko.computed(function () {
        return `code '${self.AppliedDiscountCode()}' with discount of ${self.Discount()} has been applied`;
    });
    self.IsShowDiscount = ko.computed(function () {
        return self.DiscountAmount() > 0.0;
    });
    self.OrderTotal = ko.computed(function () {
        return self.SubTotal() - self.DiscountAmount();
    });
    self.HasSubscriptions = ko.computed(function () {
        return self.UserSubscriptions().length > 0;
    });

    self.GetAvailableSubscriptions = function () {
        BaseModel.ApiCallAsync("subscriptions/available/", "GET", null)
            .then(response => {
                if (response.success) {
                    self.AvailableSubscriptions($.map(response.data, function (subscription) {
                        return new AvailableSubscriptionDataModel(subscription);
                    }));

                    self.SelectedSubscription(self.AvailableSubscriptions().find(_ => _.Title() === 'Monthly'));
                    self.GetUserSubscriptions();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.GetUserSubscriptions = function () {
        BaseModel.ApiCallAsync(`subscriptions/user/${MainVM.UserId()}`, "GET", null)
            .then(response => {
                if (response.success) {
                    self.UserSubscriptions($.map(response.data, function (subscription) {
                        return new UserSubscriptionDataModel(subscription);
                    }));

                    self.CurrentlySubscribed(self.UserSubscriptions().find(_ => _.IsCurrent()));
                    if (!self.CurrentlySubscribed() && self.SelectedSubscriptionId() !== BaseModel.Guid.Empty) {
                        self.SelectedSubscription(self.AvailableSubscriptions().find(_ => _.Id() === self.SelectedSubscriptionId()));
                        self.ShowUserSubscriptions(false);
                        self.ShowSubscribeForm(true);
                    }
                    else {
                        self.ShowUserSubscriptions(true);
                        self.ShowSubscribeForm(false);
                    }
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.ShowSubscribe = function () {
        self.ShowSubscribeForm(true);
        self.ShowUserSubscriptions(false);
    };

    self.HideSubscribe = function () {
        self.ShowSubscribeForm(false);
        self.ShowUserSubscriptions(true);
    };

    self.ValidateDiscountCode = function () {
        if (self.DiscountCode() === undefined || self.DiscountCode() === null || self.DiscountCode() === "") {
            return;
        };

        self.ValidateDiscountCodeWithCode(self.DiscountCode());
    }

    self.ValidateDiscountCodeWithCode = function (code) {
        BaseModel.ApiCallAsync(`subscriptions/discount/${code}`, "GET", null)
            .then(response => {
                if (response.success) {
                    if (response.data.isValid) {
                        self.IsShowDiscountCodeError(false);
                        self.AppliedDiscountCode(code);
                        self.AppliedDiscountCodeId(response.data.discountCodeId);
                        self.DiscountCode("");

                        if (response.data.isPercentage) {
                            self.Discount(`${response.data.discount}%`);
                            self.DiscountAmount(self.SubTotal() * (response.data.discount / 100));
                        }
                        else if (self.SubTotal() < response.data.discount) {
                            self.ClearDiscount();
                            self.IsShowDiscountCodeError(true);
                        }
                        else {
                            self.Discount(`$${response.data.discount}`);
                            self.DiscountAmount(response.data.discount);
                        }
                    }
                    else {
                        self.ClearDiscount();
                        self.IsShowDiscountCodeError(true);
                    }
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    }

    self.ClearDiscount = function () {
        self.DiscountCode("");
        self.AppliedDiscountCode("");
        self.Discount("");
        self.DiscountAmount(0.0);
    }

    self.Subscribe = function () {
        var data = {
            UserId: MainVM.UserId(),
            SubscriptionId: self.SelectedSubscription().Id(),
            DiscountCodeId: self.AppliedDiscountCodeId(),
            Total: self.OrderTotal()
        };

        BaseModel.ApiCallAsync(`subscriptions/${self.SelectedSubscription().Id()}`, "POST", JSON.stringify(data))
            .then(response => {
                if (response.success) {
                    self.SelectedSubscriptionId(BaseModel.Guid.Empty);
                    self.SelectedSubscription(null);
                    self.GetUserSubscriptions();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    }

    self.SavePaymentMethod = function () {

    };

    self.Load = function (subscriptionId) {
        self.SelectedSubscriptionId(subscriptionId);
        self.GetAvailableSubscriptions();
    };
}