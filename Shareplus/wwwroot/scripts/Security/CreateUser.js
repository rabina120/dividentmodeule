function User(data) {
    var self = this;
    self.UserName = ko.observable(data.UserName);
    self.Password = ko.observable(data.Password);
    self.ConfirmPassword = ko.observable(data.ConfirmPassword);
    self.LockUnlock = ko.observable(data.LockUnlock);
    self.UserRole = ko.observable(data.UserRole);
    self.UserType = ko.observable(data.UserType);
    self.Validdate = ko.observable(data.Validdate);
}
function UserTypes(data) {
    var self = this;
    if (data != undefined) {
        self.UserTypeName = ko.observable(data.UserTypeName);
        self.UserTypeValue = ko.observable(data.UserTypeValue);
    }
}

function UserRoles(data) {
    var self = this;
    if (data != undefined) {
        self.UserRoleName = ko.observable(data.UserRoleName);
        self.UserRoleValue = ko.observable(data.UserRoleValue);
    }
}
function UserStatuss(data) {
    var self = this;
    if (data != undefined) {
        self.UserStatusName = ko.observable(data.UserStatusName);
        self.UserStatusValue = ko.observable(data.UserStatusValue);
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

var CreateUserViewModel = function () {

    var self = this;
    self.UserName = ko.observable();
    self.Password = ko.observable();
    self.ConfirmPassword = ko.observable();
    self.LockUnlock = ko.observable();
    self.UserRole = ko.observable();
    self.Validdate = ko.observable();

    //User Type
    self.UserType = ko.observableArray([]);
    self.UserTypeName = ko.observable();
    self.UserTypeValue = ko.observable();
    self.LoadUserType = ko.observable();
    self.selectedUserType = ko.observable();


    //User Role
    self.UserRole = ko.observableArray([]);
    self.UserRoleName = ko.observable();
    self.UserRoleValue = ko.observable();
    self.LoadUserRole = ko.observable();
    self.selectedUserRole = ko.observable();


    //User Status
    self.UserStatus = ko.observableArray([]);
    self.UserStatusName = ko.observable();
    self.UserStatusValue = ko.observable();
    self.LoadUserStatus = ko.observable();
    self.selectedUserStatus = ko.observable();

    //role
    self.SelectedRole = ko.observable();
    self.RoleList = ko.observableArray([])

    self.Email = ko.observable();
    self.Pin = ko.observable();

    self.InitializeFieldVisibility = function () {
        var selectedRoleId = ko.toJS(self.selectedUserRole);
        self.selectedUserRole.notifySubscribers(selectedRoleId); // Trigger the subscribe function manually
    };
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
                        self.InitializeFieldVisibility(); 
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
    self.LoadUserRole();
    const PAYMENT_PURPOSE_ROLE_ID = 42;
    //self.selectedUserRole.subscribe(function (selectedRoleId) {
    //    // Assuming "Payment Purpose" role has a specific RoleId, e.g., 1
    //    var paymentPurposeRoleId = 42; // Replace with the actual RoleId for "Payment Purpose"

    //    if (selectedRoleId === paymentPurposeRoleId) {
    //        // Show Email and Pin fields
    //        document.getElementById('Email').closest('.form-group').style.display = 'block';
    //        document.getElementById('Pin').closest('.form-group').style.display = 'block';
    //    } else {
    //        // Hide Email and Pin fields
    //        document.getElementById('Email').closest('.form-group').style.display = 'none';
    //        document.getElementById('Pin').closest('.form-group').style.display = 'none';
    //    }
    //});
    self.selectedUserRole.subscribe(function (selectedRoleId) {
        const showEmailPin = selectedRoleId === PAYMENT_PURPOSE_ROLE_ID;
        document.getElementById('Email').closest('.form-group').style.display = showEmailPin ? 'block' : 'none';
        document.getElementById('Pin').closest('.form-group').style.display = showEmailPin ? 'block' : 'none';
    });
    self.LoadUserStatus = function () {

        $.ajax({
            type: 'POST',
            url: '/Security/User/GetUserStatus', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (result) {
                result = JSON.parse(result);

                var mappedTasks = $.map(result, function (item) {
                    return new UserStatuss(item)
                });
                mappedTasks = ko.toJS(mappedTasks);
                self.UserStatus(mappedTasks);

            },
            error: function (error) {
                console.log("failed", error)
            }
        });

    }

    self.LoadUserStatus();
    self.Validation = function () {
        var errMsg = "";
        if (Validate.empty(self.UserName())) {
            errMsg += "Username Field Cannot Be Empty.!!<br/>";
        }
        if (Validate.empty(self.Password())) {
            errMsg += "Password Field Cannot Be Empty.!!<br/>"
        }
        if (Validate.empty(self.ConfirmPassword())) {
            errMsg += "Confirm Password Field Cannot Be Empty.!!<br/>"
        }
        if (ko.toJS(self.Password()) !== ko.toJS(self.ConfirmPassword())) {
            errMsg += "Passwords Donot Match.!! <br/>"
        }
        if (Validate.empty(self.selectedUserRole())) {
            errMsg += "Select Your User Role.!!<br/>"
        }
        if (Validate.empty(self.Validdate())) {
            errMsg += "Valid Upto Cannot be Empty.!!<br/>"
        }
        else {
            var getDate = ko.toJS(self.Validdate());
            getDate = new Date(getDate).setHours(0, 0, 0, 0);
            var todayDate = new Date().setHours(0, 0, 0, 0);
            if (getDate < todayDate) {
                errMsg += "Valid Upto Cannot Be A Past Date. <br/>";
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

    self.Save = function () {

        console.log("RoleList:", ko.toJS(self.RoleList));
        console.log("Selected Role ID:", self.SelectedRole());
        if (self.Validation() == true) {
            var today = new Date();
            var dd = String(today.getDate()).padStart(2, '0');
            var mm = String(today.getMonth() + 1).padStart(2, '0');
            var yyyy = today.getFullYear();
            var todayDate = yyyy + '-' + mm + '-' + dd;

            var futureDate = new Date(new Date().setFullYear(new Date().getFullYear() + 5));
            var dd = String(futureDate.getDate()).padStart(2, '0');
            var mm = String(futureDate.getMonth() + 1).padStart(2, '0');
            var yyyy = futureDate.getFullYear();
            var futureDate = yyyy + '-' + mm + '-' + dd;

            var user = {
                'UserName': self.UserName(),
                'Password': self.Password(),
                'ConfirmPassword': self.ConfirmPassword(),
                'UserRole': self.selectedUserRole(),
                'Validdate': self.Validdate(),
                'LockUnlock': 0,
                'CreatedDate': todayDate,
                'Pw_Change_Alert_Dt': futureDate,
                'Email': self.Email(),
                'Pin': self.Pin()
            }
           
            postReq('/Security/User/CreateUser', { aTTUserProfile: user }, null, null, { redir: '/Security/User' });
        }
    }

    self.selectedUserType.subscribe(function () {

        var userType = ko.toJS(self.selectedUserType);
        if (userType == "U") {
            document.getElementById('user_role_div').style.display = "block";
        }
        else {
            document.getElementById('user_role_div').style.display = "none";
        }

    });

}

$(document).ready(function () {
    $('#simple-date1 .input-group.date').datepicker({

        format: 'yyyy-mm-dd',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true
    });
    $("#simple-date1 .input-group.date").datepicker("setDate", new Date());
    ko.applyBindings(new CreateUserViewModel());
});