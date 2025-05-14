function BlogDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.CreatedByUserId = ko.observable(data.createdByUserId || BaseModel.Guid.Empty);
    self.MainBlogImageId = ko.observable(data.mainBlogImageId || BaseModel.Guid.Empty);
    self.Name = ko.observable(data.name || "");
    self.MainImageUrl = ko.observable(data.mainImageUrl || "");
    self.Title = ko.observable(data.title || "");
    self.ShortDescription = ko.observable(data.shortDescription || "");
    self.Body = ko.observable(data.body || "");
    self.IsPosted = ko.observable(data.isPosted);
    self.PostedDate = ko.observable(data.postedDate);
    self.DateCreated = ko.observable(data.dateCreated);

    self.PostedDateDisplay = ko.computed(function () {
        if (self.PostedDate() === null || self.PostedDate() === undefined) {
            return "";
        }
        return BaseModel.FormattedDateString(self.PostedDate());
    })
    self.PostedDisplay = ko.computed(function () {
        if (self.IsPosted() && self.PostedDate()) {
            return "Posted (" + self.PostedDateDisplay() + ")";
        }
        return "Posted";
    })
    self.DateCreatedDisplay = ko.computed(function () {
        return BaseModel.FormattedDateString(self.DateCreated());
    })
};

var BlogViewModel = function () {
    var self = this;

    self.SelectedBlog = ko.observable(new BlogDataModel());
    self.Blogs = ko.observableArray([]);
    self.IsEditBlog = ko.observable(false);

    self.GetBlogs = function () {
        BaseModel.ApiCallAsync("blogs/", "GET", null)
            .then(response => {
                if (response.success) {
                    BaseModel.DataTable.Clear("blogs-datatable");
                    self.Blogs($.map(response.data, function (blog) {
                        return new BlogDataModel(blog);
                    })); 
                    BaseModel.DataTable.Refresh("blogs-datatable", 10, 0, "desc", true);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.NewBlogName = ko.observable("");
    self.CreateBlog = function () {
        var data = { Name: self.NewBlogName() };
        BaseModel.ApiCallAsync("blogs", "POST", JSON.stringify(data))
            .then(response => {
                if (response.success) {
                    self.GetBlogs();
                    self.NewBlogName("");
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.EditBlog = function (blog) {
        self.SelectedBlog(blog);
        self.IsEditBlog(true);
    }

    self.SaveBlog = function (blog) {
        var data = ko.toJSON(blog);
        BaseModel.ApiCallAsync("blogs", "PUT", data)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success, "Blog saved successfully");
                    self.SelectedBlog(new BlogDataModel());
                    self.IsEditBlog(false);
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.SaveBlogImage = function (fileList) {
        const body = new FormData();
        body.append('formFile', fileList[0]);

        BaseModel.ApiCallAsync(`blogs/${self.SelectedBlog().Id()}/images/true`, "POST", body, true)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success, "Blog image has been updated.");
                    self.SelectedBlog().MainBlogImageId(response.data.blogImageId);
                    self.SelectedBlog().MainImageUrl(response.data.blogImageUrl);
                    Dropzone.forElement('#blog-image-dropzone').removeAllFiles(true)
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.CancelEdit = function () {
        self.SelectedBlog(new BlogDataModel());
        self.IsEditBlog(false);
    }

    self.DeleteBlog = function (blog) {
        var retval = confirm("Are you sure you want to delete this blog?");
        if (retval !== true) {
            return;
        }

        BaseModel.ApiCallAsync(`blogs/${blog.Id()}`, "DELETE", null)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success, "Blog has been deleted.");
                    self.GetBlogs();
                }
                else {
                    BaseModel.Message(BaseModel.MessageLevels.Error, response.errorMessage)
                }
            })
            .catch((error) => {
                BaseModel.Message(BaseModel.MessageLevels.Error, error);
            });
    };

    self.DeleteBlogImage = function (blog) {
        var retval = confirm("Are you sure you want to delete this image?");
        if (retval !== true) {
            return;
        }

        BaseModel.ApiCallAsync(`blogs/${blog.Id()}/images/${blog.MainBlogImageId()}`, "DELETE", null)
            .then(response => {
                if (response.success) {
                    BaseModel.Message(BaseModel.MessageLevels.Success, "Blog image has been deleted.");
                    self.SelectedBlog().MainBlogImageId(BaseModel.Guid.Empty);
                    self.SelectedBlog().MainImageUrl("");
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
        self.GetBlogs();
    };
}


function PostedBlogDataModel(data) {
    var self = this;
    if (!data) data = {};

    self.Id = ko.observable(data.id || BaseModel.Guid.Empty);
    self.MainImageUrl = ko.observable(data.mainImageUrl || "");
    self.Title = ko.observable(data.title || "");
    self.CreatedBy = ko.observable(data.createdBy || "");
    self.ShortDescription = ko.observable(data.shortDescription || "");
    self.Body = ko.observable(data.body || "");
};

var PostedBlogViewModel = function () {
    var self = this;

    self.PostedBlogs = ko.observableArray([]);
    self.Blog = ko.observable(new PostedBlogDataModel());

    self.GetPostedBlogs = function () {
        BaseModel.ApiCallAsync("blogs/posted/", "GET", null)
            .then(response => {
                if (response.success) {
                    self.PostedBlogs($.map(response.data, function (blog) {
                        return new PostedBlogDataModel(blog);
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

    self.IsShowPostedBlog = ko.observable(false);
    self.GetPostedBlog = function (blogId) {
        self.IsShowPostedBlog(false);
        BaseModel.ApiCallAsync("blogs/" + blogId, "GET", null)
            .then(response => {
                if (response.success) {
                    self.Blog(new PostedBlogDataModel(response.data));
                    self.IsShowPostedBlog(true);
                }
                else {
                    window.location = "/404";
                }
            })
            .catch((error) => {
                window.location = "/404";
            });
    }

    self.Load = function () {
        self.GetPostedBlogs();
    };
}