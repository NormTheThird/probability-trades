var MainViewModel = function () {
    var self = this;

    self.PageTitle = ko.observable();
    self.UserId = ko.observable();
    self.Username = ko.observable();
    self.IsAuthenticated = ko.observable(false);
    self.IsSystemAdmin = ko.observable(false);


    self.HeaderVM = ko.observable(new HeaderViewModel(self));
    self.PostedBlogVM = ko.observable(new PostedBlogViewModel());
    self.CurrentVM = ko.observable();

    self.Clear = function () {
        self.IsAuthenticated(false);
        self.IsSystemAdmin(false);
        self.Username("");
    };

    self.Load = function () {
        self.UserId(Cookies.get('userId'));
        self.Username(Cookies.get('username'));
        self.Username() ? self.IsAuthenticated(true) : self.IsAuthenticated(false);
        self.IsSystemAdmin(Cookies.get('isAdmin') === 'true');
    };

    self.Load();
}

var HeaderViewModel = function (parent) {
    var self = this;

    self.Logout = function () {
        $.each(document.cookie.split(/; */), function () {
            var splitCookie = this.split('=');
            Cookies.remove(splitCookie[0]);
        });
        parent.Clear();
        window.location = "/";
    };
}