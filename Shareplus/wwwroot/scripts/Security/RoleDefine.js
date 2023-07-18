
var RoleDefineViewModel = function () {

    var self = this;
    self.RoleName = ko.observable()


    self.Validation = () => {
        var errMsg = "";

        if (Validate.empty(self.RoleName())) {
            errMsg += "Please Enter New Role "
        }
        if (errMsg == "") {
            return true;
        } else {
            alert('error', errMsg)
            return false;
        }
    }

    self.SaveRole = () => {
        if (self.Validation()) {
            Openloader()
            $.ajax({
                type: 'POST',
                url: '/Security/RoleDefine/SaveRole', beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                data: { 'RoleName': self.RoleName() },
                dataType: 'json',
                success: (result) => {
                    if (result.hasError) {
                        alert('error', result.message)
                    } else {
                        if (result.isSuccess) {
                            alert('success', result.message)
                        } else {
                            alert('error', result.message)
                        }
                    }
                    
                },
                error: (xhr, status, code) => {
                    alert('error', status)
                }, complete: () => {
                    self.RoleName('')
                    Closeloader()
                }
            })
        }
    }


}
$(document).ready(function () {

    ko.applyBindings(new RoleDefineViewModel());
});