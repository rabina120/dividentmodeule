$(document).ready(function () {
    $("#Submit").on('submit', function (e) {

        e.preventDefault();
        var UserName = $("#UserName").val()
        var Password = $("#Password").val()
        var NPassword = $("#NPassword").val()
        var ReNPassword = $("#ReNPassword").val()
        var errMsg = ""
        if (UserName == "") {
            errMsg += "Please Enter UserName<br/>"
        }
        if (Password == "") {
            errMsg += "Please Enter Password<br/>"
        } if (NPassword == "") {
            errMsg += "Please Enter New Password<br/>"
        } else {
            var value = NPassword
            var pattern = new RegExp(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/gm);
            if (!pattern.test(value)) {
                errMsg += "Password Must Be UpperCase, Numeric And Symbol !!!<br>";
            }
        }
        if (ReNPassword == "") {
            errMsg += "Please Enter Re New Password<br/>"
        }
        if (NPassword != ReNPassword) {
            errMsg += "Please Confirm New Password and Re Password<br/>"
        }
        if (errMsg == "") {
            var futureDate = new Date(new Date().setFullYear(new Date().getFullYear() + 5));
            var dd = String(futureDate.getDate()).padStart(2, '0');
            var mm = String(futureDate.getMonth() + 1).padStart(2, '0');
            var yyyy = futureDate.getFullYear();
            var futureDate = yyyy + '-' + mm + '-' + dd;
            $.ajax({
                type: 'POST', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                url: '/Common/ChangePassword/ChangeUserPassword',
                data: { 'UserName': UserName, 'Password': Password, 'NewPassword': NPassword,'PasswordChangeAlertDate':futureDate},
                dataType: 'json',
                success: (res) => {
                    if (!res.IsSuccess) {
                        alert("Warning!", res.Message);
                    }else {
                        alert('success', res.Message);
                        window.location = '/Security/Login';
                    }
                }, error: (error) => {
                    alert('errror', error)
                }
            })
        } else {
            alert('error', errMsg)
        }
    })
});