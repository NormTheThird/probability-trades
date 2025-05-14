function UserDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || KastleParkApp.EmptyGuid);
    self.Name = ko.observable(data.name || "");
    self.Email = ko.observable(data.email || "");
    self.Username = ko.observable(data.username || "");
    self.DiscordUsername = ko.observable(data.username || "");
    self.StripeCustomerId = ko.observable(data.stripeCustomerId || "");
    self.IsActive = ko.observable(data.isActive);
    self.IsDeleted = ko.observable(data.isDeleted);
    self.IsEmailVerified = ko.observable(data.isEmailVerified);
    self.DateCreated = ko.observable(data.dateCreated);

    self.IsAddingUserToStripe = ko.observable(false);
    self.IsDeactivatingUser = ko.observable(false);
    self.IsDeletingUser = ko.observable(false);

    self.HasStripeCustomerId = ko.computed(function () {
        console.log(self.StripeCustomerId())
        return self.StripeCustomerId() != null && self.StripeCustomerId() != "";
    });
}

var UserViewModel = function () {
    var self = this;

    self.Users = ko.observableArray([]);
    self.IsShowUsers = ko.observable(false);

    self.GetUsers = function () {
        BaseModel.ApiCallAsync("users", "GET", null)
            .then(response => {
                if (response.success) {
                    BaseModel.DataTable.Clear("users-datatable");
                    self.Users($.map(response.data, function (user) {
                        return new UserDataModel(user);
                    }));
                    BaseModel.DataTable.Refresh("users-datatable", 10, 0, "desc", true);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            })
            .finally(() => {
                self.IsShowUsers(true);
            });
    };

    self.AddUserToStripe = function (user) {
        user.IsAddingUserToStripe(true);
        BaseModel.ApiCallAsync(`stripe/customer/${user.Id()}`, "POST", null)
            .then(response => {
                if (response.success) {
                    self.GetUsers();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            })
            .finally(() => {
                user.IsAddingUserToStripe(false);
            });
    };

    self.DeactivateUser = function (user) {
        var retval = confirm(`Are you sure you want to deactivate ${user.Username()}`);
        if (!retval) {
            return;
        }

        user.IsDeactivatingUser(true);
        BaseModel.ApiCallAsync(`users/${user.Id()}/deactivate`, "PUT", null)
            .then(response => {
                if (response.success) {
                    self.GetUsers();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            })
            .finally(() => {
                user.IsDeactivatingUser(false);
            });
    };

    self.DeleteUser = function (user) {
        var retval = confirm(`Are you sure you want to delete ${user.Username()}`);
        if (!retval) {
            return;
        }

        user.IsDeletingUser(true);
        BaseModel.ApiCallAsync(`users/${user.Id()}`, "DELETE", null)
            .then(response => {
                if (response.success) {
                    self.GetUsers();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            })
            .finally(() => {
                user.IsDeletingUser(false);
            });
    };

    self.ResendConfirmationEmail = function (user) {
        BaseModel.ApiCallAsync(`mail/send-confirmation-email/${user.Email()}`, "POST", null)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success, "Verification email sent");
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
        self.GetUsers();
    };
}