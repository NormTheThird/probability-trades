function SecurityViewModel() {
    var self = this;
    self.CurrentPage = ko.observable("");
    self.Email = ko.observable("");
    self.Username = ko.observable("");
    self.DiscordUsername = ko.observable("");
    self.Password = ko.observable("");
    self.ConfirmPassword = ko.observable("");
    self.ResetId = ko.observable("");
    self.ErrorMessage = ko.observable("");
    self.Agrees = ko.observable(false);
    self.IsSubmitting = ko.observable(false);
    self.IsShowLogin = ko.observable(false);
    self.IsShowLogin.subscribe(function (value) {
        if (value) {
            self.IsShowResetPassword(false);
            self.IsShowForgotPassword(false);
        }
    });
    self.IsShowResetPassword = ko.observable(false);
    self.IsShowResetPassword.subscribe(function (value) {
        if (value) {
            self.IsShowLogin(false);
            self.IsShowForgotPassword(false);
        }
    });
    self.IsShowForgotPassword = ko.observable(false);
    self.IsShowForgotPassword.subscribe(function (value) {
        if (value) {
            self.IsShowLogin(false);
            self.IsShowResetPassword(false);
        }
    });
    self.IsForgotPasswordEmailSent = ko.observable(false);
    self.IsShowValidateDiscord = ko.observable(true);
    self.IsShowContinueRegistration = ko.observable(false);
    self.IsShowFinishedRegistration = ko.observable(false);
    self.IsShowEmailVerified = ko.observable(false);

    self.Login = function () {
        self.Clear();
        self.IsSubmitting(true);
        var data = { Username: self.Username(), Password: self.Password() };
        BaseModel.ApiCallAsync("security/authenticate", "POST", JSON.stringify(data))
            .then(response => {
                if (!response.success) { self.ErrorMessage(response.errorMessage); }
                else {
                    console.log(response);
                    Cookies.set('userId', response.userId, { expires: 7 });
                    Cookies.set('username', response.username, { expires: 7 });
                    Cookies.set('isAdmin', response.isAdmin, { expires: 7 });
                    Cookies.set('accessToken', response.accessToken, { expires: 7 });
                    Cookies.set('refreshToken', response.refreshToken, { expires: 7 });
                    window.location = "/";
                }
            })
            .catch(err => console.error(err))
            .finally(() => { self.IsSubmitting(false); });
    }

    self.ContinueRegistration = function () {
        self.CheckDiscordUsername();
    }

    self.CheckDiscordUsername = function () {
        self.Clear();
        BaseModel.ApiCallAsync(`security/discord/${self.DiscordUsername()}`, "get", null)
            .then(response => {
                if (!response.success) { self.ErrorMessage(response.errorMessage); }
                else {
                    self.IsShowValidateDiscord(false);
                    self.IsShowContinueRegistration(true);
                    self.IsShowFinishedRegistration(false);
                }
            })
            .catch(err => console.error(err));
    }

    self.ChangeDiscordUsername = function () {
        self.IsShowValidateDiscord(true);
        self.IsShowContinueRegistration(false);
        self.IsShowFinishedRegistration(false);
    };

    self.Register = function () {
        self.Clear();
        if (!$("#reg-form").valid()) {
            return false;
        };

        self.IsSubmitting(true);
        var data = {
            Email: self.Email(),
            Username: self.Username(),
            DiscordUsername: self.DiscordUsername(),
            Password: self.Password()
        };
        BaseModel.ApiCallAsync("security/register", "POST", JSON.stringify(data))
            .then(response => {
                if (!response.success) { self.ErrorMessage(response.errorMessage); }
                else {
                    self.CreateStripeCustomer(response.userId);
                    self.IsShowValidateDiscord(false);
                    self.IsShowContinueRegistration(false);
                    self.IsShowFinishedRegistration(true);
                }
            })
            .catch(err => console.error(err))
            .finally(() => { self.IsSubmitting(false); });
    };

    self.CreateStripeCustomer = function (userId) {
        BaseModel.ApiCallAsync(`stripe/customer/${userId}`, "POST", null);
    };

    self.EmailVerified = function (userId) {
        BaseModel.ApiCallAsync(`security/verified/${userId}`, "post", null)
            .then(response => {
                if (!response.success) { self.ErrorMessage(response.errorMessage); }
                else {
                    self.IsShowEmailVerified(true);
                }
            })
            .catch(err => {
                console.error(err);
            })
            .finally(() => { self.IsShowLogin(true); });
    };

    self.ShowForgotPassword = function () {
        self.IsShowForgotPassword(true);
    };

    self.SendForgotPasswordEmail = function () {
        self.IsSubmitting(true);
        var data = { Email: self.Email() };
        BaseModel.ApiCallAsync("security/forgot-password", "PUT", JSON.stringify(data))
            .then(() => { self.IsForgotPasswordEmailSent(true); })
            .catch(err => console.error(err))
            .finally(() => { self.IsSubmitting(false); });
    };

    self.ValidatePasswordReset = function () {
        BaseModel.ApiCallAsync("security/" + self.ResetId() + "/validate-password-reset", "GET", null)
            .then(response => {
                if (response.success) { self.IsShowResetPassword(true); }
                else { self.IsShowLogin(true); }
            })
            .catch(err => {
                console.error(err);
                self.IsShowLogin;
            })
            .finally(() => { self.IsSubmitting(false); });
    }

    self.ResetPassword = function () {
        self.Clear();
        if (!$("#reset-password-form").valid()) {
            return false;
        };

        self.IsSubmitting(true);
        var data = { UserId: self.ResetId(), NewPassword: self.Password() };
        BaseModel.ApiCallAsync("security/reset-password", "PUT", JSON.stringify(data))
            .then(response => {
                if (!response.success) { self.ErrorMessage(response.errorMessage); }
                else {
                    self.Password("");
                    self.IsShowLogin(true);
                }
            })
            .catch(err => console.error(err))
            .finally(() => { self.IsSubmitting(false); });
    };

    self.Clear = function () {
        self.ErrorMessage("");
    };

    self.ClearCookies = function () {
        Cookies.remove('userId');
        Cookies.remove('username');
        Cookies.remove('isAdmin');
        Cookies.remove('accessToken');
        Cookies.remove('refreshToken');
    };

    self.BindRegistrationValidation = function () {
        $("#reg-form").validate({
            ignore: ":hidden",
            rules: {
                Email: "required",
                Username: "required",
                DiscordUsername: "required",
                Password: "required",
                Agree: "required"
            },
            messages: {
                Email: "Email is required",
                Username: "Username is required",
                DiscordUsername: "Discord Username is required",
                Password: "Password is required",
                Agree: "* Please agree to the terms to sign up"
            },
            errorPlacement: function (error, element) {
                if (element.attr("name") === "Agree") {
                    error.insertAfter(element.next());
                }
                else {
                    error.insertAfter(element);
                }
            }
        });
    };

    self.BindResetPasswordValidation = function () {
        $("#reset-password-form").validate({
            rules: {
                Password: "required",
                ConfirmPassword: {
                    required: true,
                    equalTo: "#Password"
                }
            },
            messages: {
                Password: "Password is required",
                ConfirmPassword: {
                    required: "Password confirmation is required",
                    equalTo: "passwords must match"
                }
            }
        });
    };

    self.Load = function (type, id) {
        self.BindRegistrationValidation();
        self.BindResetPasswordValidation();

        if (type === "reset" && id !== "00000000-0000-0000-0000-000000000000") {
            self.ResetId(id);
            self.ValidatePasswordReset();
        }
        else if (type === "verified") {
            self.EmailVerified(id);
        }
        else {
            self.ClearCookies();
            self.IsShowLogin(true);
        }
    };
}