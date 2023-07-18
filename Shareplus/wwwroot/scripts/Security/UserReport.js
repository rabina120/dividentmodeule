function User(data) {
    var self = this;
    if (data != undefined) {
        self.UserId = ko.observable(data.UserId);
        self.UserName = ko.observable(data.UserName);
    }
}


var UserRerportViewModel = function () {

    //Users 
    self.UserId = ko.observable();
    self.UserName = ko.observable();
    self.UserList = ko.observableArray([]);
    self.SelectedUser = ko.observable();

    self.FromDate = ko.observable();
    self.ToDate = ko.observable();

    self.ToDate(formatDate(new Date))


    //global

    var isSuperAdmin = false;

    //Validation for user rights
    self.Validation = function () {
        errMsg = "";
        if (isSuperAdmin) {
            if (Validate.empty(self.SelectedUser())) {
                errMsg += "Please Select A User !!! <br/>";
            }
        }
        if (errMsg !== "") {
            toastr.error(errMsg);
            return false;
        }
        else {
            return true;
        }
    }

    //Menu that user has access to
    self.GetUserList = function () {
        Openloader()
        $.ajax({
            type: "post",
            url: '/Security/UserDetails/GetAllUsers',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                result = JSON.parse(result);
                if (result.data.length > 0) {
                    var mappedTasks = $.map(result.data, function (item) {
                        return new User(item);
                    });
                    self.UserList(mappedTasks);
                } else {
                    alert('warning',result.message)
                }
                        
                
                
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }
        });
    }
    self.GetUserRole = () => {
        Openloader()
        $.ajax({
            type: "post",
            url: '/Security/UserReport/GetUserRole',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        $('#userSelectDiv').hide()
                        isSuperAdmin = false;
                    } else {
                        GetUserList()
                        $('#userSelectDiv').show()
                        isSuperAdmin = true;
                        $('#userSelect2').attr('disabled', false)
                    }
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }
        });
    }
    self.GetUserRole()

//Update Menu Rights to user
self.Report = function (data) {
    if (self.Validation()) {
        Openloader()
        $.ajax({
            type: "POST",
            url: '/Security/UserReport/GenerateReport',
            data: {
                'UserName': isSuperAdmin ? self.SelectedUser() : null,
                'FromDate': self.FromDate(),
                'ToDate': self.ToDate(),
                'ReportType' : data
            }, beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            datatype: "json",
            success: function (result) {
                if (result.isSuccess) {
                    var fileName = result.message;
                    var a = document.createElement("a");
                    a.href = "data:application/octet-stream;base64," + result.responseData;
                    a.download = fileName;
                    a.click();
                }
                else {
                    alert('error', result.message);
                }
            },
            error: function (error) {
                alert('error', error.message)
            },
            complete: () => {
                Closeloader()
            }
        });
    }
}
}
$(document).ready(function () {
    ko.applyBindings(new UserRerportViewModel());
    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d'
    });
    $('#simple-date2 .input-group.date').datepicker({
        format: 'yyyy-mm-dd',
        todayHighlight: true,
        autoclose: true,
        endDate: '+0d'
    });


    $('#userSelectDiv').hide()
});