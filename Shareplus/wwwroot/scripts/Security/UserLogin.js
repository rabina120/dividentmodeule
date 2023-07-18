$('#LDAPDiv').hide();
var checkConnection = false;

checkConnectionString = () => {
    $.ajax({
        type: 'POST',
        
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        url: '/Security/Login/CheckConnectionString',
        success: function (result) {
            if (result.isSuccess) {
                checkConnection = true;
                $('#SecurityDatabaseModal').modal('hide')
            } else {
                if (checkConnection == true) {
                    $('#SecurityDatabaseModal').modal('hide')
                } else {
                    $('#SecurityDatabaseModal').modal('show')
                }
            }
        }, error: (err) => {
            alert('error', err.message)
        }, complete: () => {

        }
    })
}
if (checkConnection == false) {
    checkConnectionString()
}

$("#Submit").on('submit', function (e) {
    e.preventDefault();
    UserId = $("#UserName").val();
    Password = $("#Password").val();
    errMsg = "";
    if (UserId == "") {
        errMsg += "Please Enter UserName <br />"
    }
    if (Password == "") {
        errMsg += "Please Enter Password <br />"
    }
    if (errMsg == "") {
        Openloader()
        $.ajax({
            type: 'POST',
            url: '/Security/Login', 
            data: {  'UserId': UserId, 'Password': Password },
            dataType: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    if (true) { //was for checking password result.responseData.hasPswChanged
                        alert('success', 'Login Successfully ')
                        swal(result.message)
                        localStorage.removeItem('company-code')
                        localStorage.removeItem('company-name')
                        swal({
                            title: "LAST LOGIN INFORMATION",
                            text: result.message,
                            type: "success"
                        }).then(function () {
                            window.location = '/Common/Dashboard';
                        });
                    } else {
                        alert('warning', 'Please Change Your Password')
                        var userid = result.responseData.userId;
                        var username = result.responseData.userName;
                        window.location.href = '/Common/ChangePassword?UserName=' + username;
                    }
                } else {
                    alert('info', result.message)
                }
            },
            error: (error) => {
                alert('error', error)
            },
            complete: () => {
                Closeloader()
            }
        });
    }
    else {
        alert('error', errMsg)
    }
});

EnableLDAP= ()=> {
    document.querySelector('#isLDAP').checked ? $('#LDAPDiv').show() : $('#LDAPDiv').hide();
}
$("#ServerSubmit").on('submit', function (e) {
    e.preventDefault();
    ServerName = $("#ServerName").val();
    ServerNamePort = $("#ServerNamePort").val();
    DatabaseName = $("#DatabaseName").val();
    ServerUserName = $("#SUserName").val();
    ServerPassword = $("#SPassword").val();
    isLDAP = document.querySelector('#isLDAP').checked ? 1 : 0;
    LDAPServer = $("#LDAPServer").val();
    errMsg = "";
    if (ServerName == "") {
        errMsg += "Please Enter ServerName <br />"
    }

    if (DatabaseName == "") {
        errMsg += "Please Enter DatabaseName <br />"
    }
    if (ServerUserName == "") {
        errMsg += "Please Enter Server User Name <br />"
    }
    if (ServerPassword == "") {
        errMsg += "Please Enter Server Password <br />"
    }
    if (isLDAP == 1 && LDAPServer == "") {
        errMsg += "Please Enter LDAP Server Address <br />"
    }
    if (errMsg == "") {
        Openloader()

        $.ajax({
            type: 'POST',
            url: '/Security/Login/SetConnectionString',
            data: {'ServerName': ServerName, 'Port': ServerNamePort, 'DatabaseName': DatabaseName, 'ServerUserName': ServerUserName, 'ServerPassword': ServerPassword,'LDAPServer':LDAPServer },
            dataType: 'json', beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: (result) => {
                if (result.isSuccess) {
                    checkConnection = true;
                    swal({
                        text: result.message
                    }).then(function () {
                        window.location.reload();
                        $("#ServerName").val() = "";
                        $("#ServerNamePort").val() = "";
                        $("#DatabaseName").val() = "";
                        $("#SUserName").val() = "";
                        $("#SPassword").val() = "";
                        window.location = '/Security/Login'

                    });
                } else {
                    swal({
                        text: result.message
                    }).then(function () {
                        window.location.reload();
                        $("#ServerName").val() = "";
                        $("#DatabaseName").val() = "";
                        $("#SUserName").val() = "";
                        $("#SPassword").val() = "";
                        window.location = '/Security/Login'
                    });
                }
            },
            error: (error) => {
                alert('error', error)
            },
            complete: () => {
                Closeloader()
            }
        });

    }
    else {
        alert('warning', errMsg)
    }
});

