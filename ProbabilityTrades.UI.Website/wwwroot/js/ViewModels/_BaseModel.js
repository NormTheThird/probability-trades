var BaseModel = {
    BaseApiUrl: "https://probability-trades-api.azurewebsites.net/api/",
    ApiCallAsync: async function (url, method, data, isFormData) {
        const options = {
            method: method,
            body: data,
            headers: BaseModel.GetApiCallHeaders(Cookies.get("accessToken"), isFormData)
        }

        return await BaseModel.FetchRetry(BaseModel.BaseApiUrl + url, options)
            .then((response) => response.json());
    },
    FetchRetry: function (url, options) {
        return fetch(url, options)
            .then(response => {
                if (response.status == 401) {
                    const refreshOptions = {
                        method: "POST",
                        body: JSON.stringify({ RefreshToken: Cookies.get('refreshToken') }),
                        headers: {
                            "Accept": "application/json, text/plain, */*"
                        }
                    };
                    return fetch(BaseModel.BaseApiUrl + "security/refresh-authentication", refreshOptions)
                        .then((response) => response.json())
                        .then((data) => {
                            if (data.success) {
                                var refreshToken = data.refreshToken;
                                Cookies.set('accessToken', data.accessToken, { expires: 7 });
                                Cookies.set('refreshToken', refreshToken, { expires: 7 });
                                options.headers = {
                                    "Authorization": "Bearer " + Cookies.get("accessToken"),
                                    "Accept": "application/json, text/plain, */*"
                                }
                                return BaseModel.FetchRetry(url, options);
                            }
                            else {
                                window.location = "/security";
                            }
                        });
                }
                else {
                    return response;
                }
            });
    },
    GetApiCallHeaders: function (accessToken, isFormData) {
        var headers = {
            "Accept": "application/json, text/plain, *",
            "Access-Control-Allow-Origin": "*",
            "Access-Control-Allow-Methods": "GET, POST, PUT, PATCH, DELETE, OPTIONS"
        }

        if (accessToken) { headers["Authorization"] = "Bearer " + accessToken; }

        if (isFormData === undefined || isFormData === false) { headers["Content-Type"] = "application/json;charset=utf-8"; }

        return headers;
    },
    Message: function (level, message) {
        if (level.type === "success")
            toastr.success(message, "Success", { timeOut: 1500, progressBar: false });
        else if (level.type === "info")
            toastr.info(message, "Information", { timeOut: 1500, progressBar: false });
        else if (level.type === "warning")
            toastr.warning(message, "Warning", { timeOut: 0 });
        else if (level.type === "error")
            toastr.error(message, "Error", { timeOut: 0 });
    },
    MessageLevels: {
        Success: { type: "success", color: "green" },
        Info: { type: "info", color: "blue" },
        Warning: { type: "warning", color: "yellow" },
        Error: { type: "error", color: "red" }
    },
    LogError: function (exception) {
        console.error(exception);
        BaseModel.Message(exception, BaseModel.MessageLevels.Error);
    },
    GetUrlQuery: function (query) {
        var args = window.location.href.split('?');
        var params = {};
        if (args.length <= 1) { return null; }
        var url = args[1].split('#')[0].split("&");
        for (i in url) {
            param = url[i].split("=");
            params[param[0].toLowerCase()] = param[1];
        }
        if (query) {
            return params[query];
        }
        return params;
    },
    Guid: {
        Empty: '00000000-0000-0000-0000-000000000000',
        New: function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
        }
    },
    ToDate: function (date, format) {
        try {
            if (date) {
                if (date instanceof Date) { return date; }
                var newDate = new Date(parseInt(date.match(/\d+/) || 0));
                if (format === "mm/dd/yyyy") return newDate.toLocaleDateString();
                if (format === "yyyy-dd-mm") return newDate.toISOString().split('T')[0];
                return newDate;
            }
        }
        catch (ex) {
            BaseModel.LogError("Date parse failed", date);
        }
    },
    FormattedDateString: function (string) {
        if (string == undefined || string.length < 8) {
            var today = new Date();
            return today.toLocaleDateString();
        }

        //need to offset for time zone
        if (typeof (string) === "string") {
            if (string.indexOf("0001-01-01") > -1) { return "01/01/2001" }
            var d = new Date();
            var m = d.getTimezoneOffset();
            var hString = ((m + 60) / 60).toString();
            if (hString.length < 2) { hString = "0" + hString; }
            string = string.replace("T00", "T" + hString);
        }
        var date = new Date(string);
        var result = ('0' + (date.getUTCMonth() + 1)).slice(-2) + '/' + ('0' + date.getUTCDate()).slice(-2) + '/' + date.getUTCFullYear();
        return result;
    },
    ToTime: function (date) {
        try {
            if (date) {
                if (date instanceof Date) { return date; }
                var newDate = new Date(parseInt(date.match(/\d+/) || 0));
                var minutes = newDate.getMinutes();
                if (minutes < 10)
                    minutes = "0" + minutes;
                var hour = newDate.getHours();
                if (hour > 12)
                    return ((hour - 12) + ":" + minutes + " PM")
                else if (hour == 12)
                    return (hour + ":" + minutes + " PM")
                else
                    return (hour + ":" + minutes + " AM")
            }
        }
        catch (ex) {
            console.error("Date parse Failed", date);
        }
    },
    States: [
        { Name: "Alabama", ShortName: "AL" },
        { Name: "Alaska", ShortName: "AK" },
        { Name: "Arizona", ShortName: "AZ" },
        { Name: "Arkansas", ShortName: "AR" },
        { Name: "California", ShortName: "CA" },
        { Name: "Colorado", ShortName: "CO" },
        { Name: "Connecticut", ShortName: "CT" },
        { Name: "Delaware", ShortName: "DE" },
        { Name: "District of Columbia", ShortName: "DC" },
        { Name: "Florida", ShortName: "FL" },
        { Name: "Georgia", ShortName: "GA" },
        { Name: "Guam", ShortName: "GU" },
        { Name: "Hawaii", ShortName: "HI" },
        { Name: "Idaho", ShortName: "ID" },
        { Name: "Illinois", ShortName: "IL" },
        { Name: "Indiana", ShortName: "IN" },
        { Name: "Iowa", ShortName: "IA" },
        { Name: "Kansas", ShortName: "KS" },
        { Name: "Kentucky", ShortName: "KY" },
        { Name: "Louisiana", ShortName: "LA" },
        { Name: "Maine", ShortName: "ME" },
        { Name: "Maryland", ShortName: "MD" },
        { Name: "Massachusetts", ShortName: "MA" },
        { Name: "Michigan", ShortName: "MI" },
        { Name: "Minnesota", ShortName: "MN" },
        { Name: "Mississippi", ShortName: "MS" },
        { Name: "Missouri", ShortName: "MO" },
        { Name: "Montana", ShortName: "MT" },
        { Name: "Nebraska", ShortName: "NE" },
        { Name: "Nevada", ShortName: "NV" },
        { Name: "New Hampshire", ShortName: "NH" },
        { Name: "New Jersey", ShortName: "NJ" },
        { Name: "New Mexico", ShortName: "NM" },
        { Name: "New York", ShortName: "NY" },
        { Name: "North Carolina", ShortName: "NC" },
        { Name: "North Dakota", ShortName: "ND" },
        { Name: "Ohio", ShortName: "OH" },
        { Name: "Oklahoma", ShortName: "OK" },
        { Name: "Oregon", ShortName: "OR" },
        { Name: "Pennsylvania", ShortName: "PA" },
        { Name: "Rhode Island", ShortName: "RI" },
        { Name: "South Carolina", ShortName: "SC" },
        { Name: "South Dakota", ShortName: "SD" },
        { Name: "Tennessee", ShortName: "TN" },
        { Name: "Texas", ShortName: "TX" },
        { Name: "Utah", ShortName: "UT" },
        { Name: "Vermont", ShortName: "VT" },
        { Name: "Virginia", ShortName: "VA" },
        { Name: "Washington", ShortName: "WA" },
        { Name: "West Virginia", ShortName: "WV" },
        { Name: "Wisconsin", ShortName: "WI" },
        { Name: "Wyoming", ShortName: "WY" }
    ],
    CandlestickPatterns: [
        { Name: "Fifteen Minute", Value: "FifteenMinute" },
        { Name: "One Hour", Value: "OneHour" },
        { Name: "One Day", Value: "OneDay" }
    ],
    Currency: [
        { Name: "BTC", Value: "BTC" },
        { Name: "ETH", Value: "ETH" },
        { Name: "USDT", Value: "USDT" }
    ],
    MovingAverageDays: [
        { Name: "3", Value: "3" },
        { Name: "5", Value: "5" },
        { Name: "8", Value: "8" },
        { Name: "9", Value: "9" },
        { Name: "13", Value: "13" },
        { Name: "21", Value: "21" },
        { Name: "34", Value: "34" },
        { Name: "50", Value: "50" },
        { Name: "55", Value: "55" },
        { Name: "89", Value: "89" },
        { Name: "144", Value: "144" },
        { Name: "233", Value: "233" }
    ],
    Environment: function () {
        var baseUrl = window.location.hostname;
        if (baseUrl.match("tek-find.azurewebsites.net")) { return "PROD"; }
        else if (baseUrl.match("Yoonite.io")) { return "PROD"; }
        else if (baseUrl.match("tek-find-test.azurewebsites.net")) { return "TEST"; }
        else { return "DEV"; }
    },
    DataTable: {
        Clear: function (domId) {
            if ($.fn.DataTable.isDataTable('#' + domId)) {
                $('#' + domId).dataTable().fnClearTable();
                $('#' + domId).dataTable().fnDestroy();
            }
        },
        Refresh: function (domId, pageLength, sortColumn, sortDirection, showSearch) {
            var dt = $('#' + domId).dataTable({
                "order": [[sortColumn ?? 0, sortDirection ?? "asc"]],  // Allows ordering
                "dom": '<"top pull-left mt-15 mb-15"f>rt<"bottom"lp><"clear">',              // Positions table elements
                "searching": showSearch,                               // Searchbox
                "paging": pageLength > 0,                              // Pagination
                "pageLength": pageLength,                              // Defaults number of rows to display in table
                "pagingType": "full_numbers",                          // Shows first, previous, page numbers, next and last buttons
                "retrieve": true,
                "columnDefs": [{
                    "targets": sortColumn,
                    "type": 'date',
                }],
                "language": {
                    "search": "_INPUT_",                               // Removes the 'Search' field label
                    "searchPlaceholder": "Search"                      // Placeholder for the search box
                }
            });
            return dt;
        }
    },
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        var options = {
            showOtherMonths: true,
            electOtherMonths: true,
            format: 'mm/dd/yyyy',
            showOn: "both",
            todayBtn: 'linked',
            todayHighlight: true,
            showButtonPanel: true
        };

        if (allBindings.has("mindate")) {
            options.startDate = allBindings.get("mindate");
        }

        if (allBindings.has("maxDate")) {
            options.endDate = allBindings.get("maxDate");
        }

        if (typeof valueAccessor() === 'object') {
            $.extend(options, valueAccessor());
        }

        $(element).datepicker(options);
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        if (allBindings.has("type")) {
            var type = allBindings.get("type");
            if (type = 'MonthYear' && allBindings.has("isEndOfMonth")) {
                var isEndOfMonth = allBindings.get("isEndOfMonth");
                if (isEndOfMonth && value !== undefined && value !== null) {
                    var selectedDate = new Date(value);
                    var newDate = new Date(selectedDate.getFullYear(), selectedDate.getMonth() + 1, 0)
                    value = (newDate.getMonth() + 1) + '-' + newDate.getDate() + '-' + newDate.getFullYear();
                }
            }
        }

        $(element).datepicker('setDate', value);
    }
};

// In this order
// element: The bound element in the dom
// valueAccessor: The observable you are bound to
// allBindingsAccessor: other bindings in the same element
// viewModel: you are bound to
// bindingContext: 
ko.bindingHandlers.newBindingHandler = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        console.log("newBindingHandler Init:")
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        console.log("newBindingHandler Update:")
    }
};


Dropzone.autoDiscover = false;
//var dropzone;
ko.bindingHandlers.dropzone = {
    init: function (element, valueAccessor) {
        var value = ko.unwrap(valueAccessor());
        var options = {
            //maxFiles: 1,
            uploadMultiple: false,
            createImageThumbnails: true,
            autoProcessQueue: false
        };

        $.extend(options, value);
        $(element).addClass("dropzone");
        dropzone = new Dropzone(element, options);
        dropzone.on("queuecomplete", function () {
            dropzone.removeAllFiles(true);
        });
    },
    update: function (element, valueAccessor) {

    }
};

//ko.subscribable.fn.PhoneMask = PhoneMask;
//function PhoneMask() {
//    return ko.computed({
//        read: function () {
//            var phoneNumber = this() ? this() : "";
//            if (phoneNumber.length < 1) return;
//            var out = "";
//            out = out + "(" + phoneNumber.substring(0, 3);
//            if (phoneNumber.length > 3) out = out + ") " + phoneNumber.substring(3, 6);
//            if (phoneNumber.length > 6) out = out + "-" + phoneNumber.substring(6, 10);
//            return out;
//        },
//        write: function (_phoneNumber) {
//            var phoneNumber = _phoneNumber.replace(/[^\.\d]/g, "");
//            this(phoneNumber);
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.ToAmount = ToAmount;
//function ToAmount() {
//    return ko.computed({
//        read: function () {
//            var num = ko.unwrap(this);
//            return this() ? "$" + num.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",") : "$0.00";
//        },
//        write: function (_amount) {
//            var amount = (_amount.match(/\d+\.?/g) || [0]).join("");
//            this(parseFloat(amount));
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.ToShortAmount = ToShortAmount;
//function ToShortAmount() {
//    return ko.computed({
//        read: function () {
//            var num = ko.unwrap(this);
//            return this() ? "$" + num.toFixed(0).replace(/\B(?=(\d{3})+(?!\d))/g, ",") : "$0";
//        },
//        write: function (_amount) {
//            this(_amount);
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.OnlyPositive = OnlyPositive;
//function OnlyPositive() {
//    return ko.computed({
//        read: function () {
//            return this();
//        },
//        write: function (_amount) {
//            this(Math.abs(_amount));
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.ToShortDate = ToShortDate;
//function ToShortDate() {
//    return ko.computed({
//        read: function () {
//            if (this() instanceof Date) {
//                return this().toLocaleDateString('en-US', { month: 'numeric', day: 'numeric', year: 'numeric' });
//            }
//        },
//        write: function (date) {
//            if (date) {
//                this(new Date(date));
//            }
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.ToDaysAgo = ToDaysAgo;
//function ToDaysAgo() {
//    return ko.computed({
//        read: function () {
//            if (this() instanceof Date) {
//                var today = new Date();
//                return Math.floor((today - this()) / 1000 / 60 / 60 / 24);
//            }
//        },
//        write: function (date) {
//            if (date) {
//                this(new Date(date));
//            }
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//ko.subscribable.fn.ToEin = ToEin;
//function ToEin() {
//    return ko.computed({
//        read: function () {
//            var ein = this() ? this() : "";
//            if (ein.length < 3) return ein;
//            var out = "";
//            if (ein.length > 2) {
//                out = ein.substr(0, 2) + "-" + ein.substr(2, 7);
//            }
//            return out;
//        },
//        write: function (_ein) {
//            var ein = _ein.replace(/[^\.\d]/g, "");
//            this(ein);
//            this.notifySubscribers();
//        },
//        owner: this,
//    }).extend({ notify: 'always' });
//}

//function dataURItoBlob(dataURI) {
//    // convert base64 to raw binary data held in a string
//    // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
//    var byteString = atob(dataURI.split(',')[1]);

//    // separate out the mime component
//    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0]

//    // write the bytes of the string to an ArrayBuffer
//    var ab = new ArrayBuffer(byteString.length);

//    // create a view into the buffer
//    var ia = new Uint8Array(ab);

//    // set the bytes of the buffer to the correct values
//    for (var i = 0; i < byteString.length; i++) {
//        ia[i] = byteString.charCodeAt(i);
//    }

//    // write the ArrayBuffer to a blob, and you're done
//    var blob = new Blob([ab], { type: mimeString });
//    return blob;
//}
