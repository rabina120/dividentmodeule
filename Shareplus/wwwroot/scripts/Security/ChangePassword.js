var ChangePasswordViewModel = function () {
    var self = this;
    self.Password = ko.observable('');
    self.ConfirmPassword = ko.observable('');
    self.ReConfirmPassword = ko.observable('');


    self.Validation = function () {
        debugger;
        var errMsg = "";
       
        if (Validate.empty(self.Password())) {
            errMsg += "Current Password Field Cannot Be Empty.<br/>"
        }

        if (Validate.empty(self.ConfirmPassword())) {
            errMsg += "Password Field Cannot Be Empty.<br/>"
        } else {
            value = ko.toJS(self.Password())
            var pattern = new RegExp(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$/gm);
            if (!pattern.test(value)) {
                errMsg += "Password Must Be UpperCase, Numeric n Symbol !!!<br>";
            }
        }
        if (Validate.empty(self.ReConfirmPassword())) {
            errMsg += "Confirm Password Field Cannot Be Empty.<br/>"
        }

        if (ko.toJS(self.ConfirmPassword()) != ko.toJS(self.ReConfirmPassword())) {
            errMsg += "Passwords Donot Match. <br/>"
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
        if (self.Validation()) {
            var futureDate = new Date(new Date().setFullYear(new Date().getFullYear() + 5));
            var dd = String(futureDate.getDate()).padStart(2, '0');
            var mm = String(futureDate.getMonth() + 1).padStart(2, '0');
            var yyyy = futureDate.getFullYear();
            var futureDate = yyyy + '-' + mm + '-' + dd;
        postReq('/Security/ChangePassword/ChangeUserPassword', { Password: self.Password(), NewPassword: self.ConfirmPassword(),PasswordChangeAlertDate: futureDate }, null, null, { redir: '/Security/Login/Logout' });
    }}
}
$(document).ready(function () {
    ko.applyBindings(new ChangePasswordViewModel());
});