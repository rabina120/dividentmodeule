
var dataTable;

$(document).ready(function () {
    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());
    $('#simple-date3 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date3 .input-group.date").datepicker("setDate", new Date());
    $('#simple-date4 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date4 .input-group.date").datepicker("setDate", new Date());
    ko.applyBindings(new UserDetails());
});


self.Rights = function (data) {
    var self = this;
    if (data != undefined) {
        self.MenuID = ko.observable(data.menuId);
        self.MenuText = ko.observable(data.menuText);
        self.Parent = ko.observable(data.parentId == "0" ? true : false)
        self.UserName = ko.observable(data.userName)
    }
}
function Role(data) {
    var self = this;
    if (data != undefined) {
        self.RoleId = ko.observable(data.roleId);
        self.RoleName = ko.observable(data.roleName);
        self.IsActive = ko.observable(data.isActive);
    }
}
self.Edits = function (data) {
    var self = this;
    if (data != undefined) {
        self.UserName = ko.obsevable(data.userName)
        self.UserRoles = ko.observable(data.UserRoles)
        self.ValidateDate = ko.observable(data.ValidateDate)
    }
}
function User(data) {
    var self = this;
    if (data != undefined) {
        self.UserIdForReport = ko.observable(data.UserId);
        self.UserNameForReport = ko.observable(data.UserName);
    }
}
var UserDetails = function () {

    //Users 
    self.UserIdForReport = ko.observable()
    self.UserNameForReport = ko.observable()
    self.UserList = ko.observableArray([]);
    self.SelectedUser = ko.observable();

    self.UserId = ko.observable()
    self.UserName = ko.observable()
    self.Password = ko.observable()
    self.CPassword = ko.observable()
    self.ValidUpto = ko.observable()
    self.LockUnlock = ko.observable(false)
    self.SelectedRole = ko.observable();
    self.RoleList=ko.observableArray([])
    self.RightsList = ko.observableArray([])
    self.fromDate = ko.observable();
    self.toDate = ko.observable();
    loadDataTable = function () {
        dataTable = $('#DT_load').dataTable({
            "ajax": {
                "url": "/Security/UserDetails/GetAllUsers",
                "type": "POST", beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                "datatype": "json"
            },
            "columns": [
                { "data": "UserId", "width": "5%" },
                { "data": "UserName", "width": "15%" },
                { "data": "UserId", "width": "5%" },
                { "data": "UserType", "width": "10%" },
                {
                    "data": "LastLoggedInDateTime",
                    "render": (data) => {
                        var LastLoggedInDateTimeString = data.substring(0, 10) + " ; " + data.substring(11, 19)
                        return LastLoggedInDateTimeString
                    },
                    "width": "15%"
                },
                { "data": "AccountType", "width": "15%" },
                {
                    "data": "LockUnlock", "render": (data) => {
                        if (!data)
                            return "Unlocked"
                        else
                            return "Locked"

                    }, "width": "15%"
                },
                {
                    "data": "UserId",
                    "render": function (data) {
                        return `<div class="text-center">
                        <a  onclick=ShowRightsModal(${data}) class='btn btn-danger text-white' style='cursor:pointer;'>
                            Rights
                        </a>
                        </div>`;
                    },
                    "width": "10%"
                }, {
                    "data": "UserId",
                    "render": function (data, type, row, meta) {
                        if (row.IsDeleted) {
                            return `<div class="text-center">
                        <a  onclick=Delete("/Security/UserDetails/EnableUserById?id=${data}",false) class='btn btn-success text-white' style='cursor:pointer;'>
                            Enable
                        </a>
                        </div>`;
                        }
                        else {
                            return `<div class="text-center">
                        <a  onclick=Delete("/Security/UserDetails/DisableUserById?id=${data}",true) class='btn btn-danger text-white' style='cursor:pointer;'>
                            Disable
                        </a>
                        </div>`;
                        }
                        
                    },
                    "width": "10%"
                }, {
                    "data": "UserId",
                    "render": function (data,row) {
                       
                        return `<div class = "text-center">
                        <a onclick=Edit("/Security/UserDetails/EditUserById?id=${data}") class='btn btn-primary text-white' style='cursor:pointer;'>
                            Edit
                        </a>
                        </div>`;
                    },
                    "width":"10%"
                }
            ],
            "dom": 'Bfrtip',
            "buttons": [
                {
                    extend: 'excel',
                    exclude: ".Preview",
                    filename: function () {
                        var dateObj = new Date();
                        var month = dateObj.getUTCMonth() + 1; //months from 1-12
                        var day = dateObj.getUTCDate();
                        var year = dateObj.getUTCFullYear();
                        newdate = year + "/" + month + "/" + day;
                        return "User Details" + newdate;
                    },
                    exportOptions: {
                        orthogonal: 'sort',
                        columns: [0, 1, 2, 3, 4, 5, 6]
                    },
                }
            ],
            "aoColumnDefs": [
                {
                    "bSortable": false,
                    "aTargets": [-1]
                }
            ],
            "language": {
                "emptyTable": "no data found"
            }
        });
    }

    loadDataTable();

    self.GetUserList = function () {
        $.ajax({
            type: "post",
            url: '/Security/UserDetails/GetAllUsers',
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                result = JSON.parse(result);
                var mappedTasks = $.map(result.data, function (item) {
                    return new User(item);
                });
                self.UserList(mappedTasks);



            },
            error: function (error) {
                alert("error", error)
            }
        });
    }
   
    self.PDFAuditReport = function () {
        self.AuditReport('pdf');
    }

    self.ExcelAuditReport = function () {
        self.AuditReport('excel');
    }

    self.ShowModalReport = () => {
        $('#ReportModal').modal('show');
        self.GetUserList();
    }
    self.AuditReport = (type) => {
        $.ajax({
            url: '/Security/UserDetails/GetAuditReport', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            }, type: 'post',
            data: {'reportType':type,'fromDate':self.fromDate(),'toDate':self.toDate(),'userId':self.SelectedUser()},
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
                console.log("failed", error)
            }
        });
    }
    self.getLocation = function (data, event) {
        return ko.utils.arrayFirst(self.RoleList(), function (location) {
            return location.toLowerCase() === data.toLowerCase();
        });
    };
    self.Edit = function (url) {
       
        self.ClearControls();
        self.LoadUserRole();
        $.ajax({
            type: "post",
            url: url,
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (data) {
                if (data.isSuccess) {
                     self.UserName(data.responseData.UserName)
                    self.UserId(data.responseData.userId)
                    self.UserName(data.responseData.userName)
                    self.LockUnlock(data.responseData.lockUnlock)
                    self.SelectedRole(ko.toJS(self.RoleList()).find(x=> x.RoleId==data.responseData.userRole)?.RoleId);
                    self.ValidUpto(data.responseData.validdate.substring(0,10))
                    
                    if (data.isValid) {
                        $('.password').hide();
                    }
                   
                }
                else {
                    toastr.error("Record No Found");
                }
            }
        });
        $('#exampleModalCenter').modal('show');

    }
  
    self.LoadUserRole = function () {
        $.ajax({
            url: '/Security/UserDetails/GetAllRoles', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                if (result.hasError) {
                    alert('error', result.message)
                } else {
                    if (result.isSuccess) {
                        var mappedTasks = $.map(result.responseData, function (item) {
                            return new Role(item);
                        });
                        self.RoleList(mappedTasks);
                    }
                    else {
                        alert('warning', result.message)
                    }


                }


            },
            error: function (error) {
                console.log("failed", error)
            }
        });

    }
   

    self.Validation = function () {

        var errMsg = "";
        if (!Validate.empty(self.Password())) {
        
            if (self.Password() != self.CPassword()) {
                errMsg += "Passwords Must Match.<br/>"
            }
        }

        if (Validate.empty(self.ValidUpto())) {
            errMsg += "Valid Date Cannot be Empty.<br/>"
        }
        else {
            var getDate = ko.toJS(self.ValidUpto);
            getDate = new Date(getDate).setHours(0, 0, 0, 0);
            var todayDate = new Date().setHours(0, 0, 0, 0);
            if (getDate < todayDate) {
                errMsg += "Date Cannot Be A Past Date. <br/>";
            }

        }

        if (errMsg !== "") {

            alert("error", errMsg);

            return false;
        }
        else {
            return true;
        }
    }

    self.UpdateDetails = function () {
        var futureDate = new Date(new Date().setFullYear(new Date().getFullYear() + 5));
        var dd = String(futureDate.getDate()).padStart(2, '0');
        var mm = String(futureDate.getMonth() + 1).padStart(2, '0');
        var yyyy = futureDate.getFullYear();
        var futureDate = yyyy + '-' + mm + '-' + dd;
        var data = {
            'Password': ko.toJS(self.Password()),
            'Validdate': ko.toJS(self.ValidUpto()),
            'UserName': ko.toJS(self.UserName()),
            'LockUnlock': ko.toJS(self.LockUnlock()),
            'Pw_Change_Alert_Dt': futureDate,
            'UserRole':ko.toJS(self.SelectedRole())
        }
        if (self.Validation()) {
            swal({
                title: "Are you sure?",
                text: `User Id ${self.UserId()} User Details will be Change`,
                icon: "warning",
                buttons: true,
                dangerMode: true
            }).then((willDelete) => {
                if (willDelete) {
                    $.ajax({
                        type: "post",
                        url: `/Security/UserDetails/UpdateUserDetails`,
                        data: { 'id': self.UserId(), 'UserProfile': ko.toJS(data) },
                        datatype: "json", beforeSend: function (xhr) {
                            xhr.setRequestHeader("XSRF-TOKEN",
                                $('input:hidden[name="__RequestVerificationToken"]').val());
                        },
                        success: function (data) {
                            if (data.isSuccess) {
                                toastr.success(data.message);
                                $('#DT_load').DataTable().destroy()
                                loadDataTable();
                            }
                            else {
                                toastr.error(data.message);

                            }
                        }
                    });
                }
            });
            $('#exampleModalCenter').modal('hide');
        }
    }

    self.ClearControls = function () {
        self.Password('')
        self.ValidUpto('')
        self.UserName('')
        self.UserId('')
        self.LockUnlock('')

    }

    self.Delete = function (url,del) {

        swal({
            title: "Are you sure?",
            text: del ? "Once disabled, user wont be able to login" : "Once Enabled, user will be able to login",
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: "POST",
                    url: url,
                    datatype: "json", beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
                    success: function (data) {
                        //data = JSON.parse(data)
                        if (data.isSuccess) {
                            toastr.success(data.message);
                            $('#DT_load').DataTable().destroy()
                            loadDataTable();
                        }
                        else {
                            toastr.error(data.message);

                        }
                    }
                });
            }
        });
    }

    self.ShowRightsModal = function (data) {
        $.ajax({
            type: "POST",
            url: `/Security/UserDetails/GetUserRights`,
            data: { 'UserID': data },
            datatype: "json", beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (data) {
                if (data.isSuccess) {
                    $('#rightsDataTable').DataTable().destroy();

                    var mappedTasks = $.map(data.responseData, function (item) {
                        return new Rights(item);
                    });
                    self.RightsList(mappedTasks);
                    $('#rightsDataTable').DataTable({
                        "dom": 'Bfrtip',
                        "buttons": [
                            {
                                extend: 'excel',
                                include: ".Preview",
                                filename: function () {
                                    return "User Rights" + data.responseData[0].userName;
                                }
                            }
                        ],
                    })
                    $('#RightsModal').modal('show')
                }
                else {
                    
                }
            }
        });
    }
}